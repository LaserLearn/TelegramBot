using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.contract.Bot.Toggele;
using TelegramBot.model.Enum;
using TelegramBot.Tool;

namespace TelegramBot.handlers.Toggle
{
    public class ToggleSelectionHandler : IToggleHandlers
    {
        private readonly TelegramBotClient _botClient;
        private const int ButtonCount = 8;
        public ToggleSelectionHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task<bool> ShouldHandleAsync(Update update)
        {
            long? chatId = UserStateTools.GetChatId(update);

            if (chatId == null)
                return false;

            var step = await UserStateTools.GetStateAsync(chatId.Value);

            if (update.CallbackQuery != null && update.CallbackQuery.Data == "CheckMark" ||
                await UserStateTools.GetMotherStateAsync(chatId) == nameof(UserBotState.InCheckMark))
            {
                await UserStateTools.SetMotherAsync(chatId: chatId, nameof(UserBotState.InCheckMark));
                return true;
            }
            return false;
        }

        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {
            await SendInitialToggleMessage(update, cancellationToken);
        }

        public async Task SendInitialToggleMessage(Update update, CancellationToken cancellationToken)
        {
            long? chatId = UserStateTools.GetChatId(update);
            if (chatId == null)
                return;

            var messageId = update.CallbackQuery?.Message?.MessageId;
            string? callbackData = update.CallbackQuery?.Data;

            var state = await UserStateTools.GetDataAsync(chatId.Value, "toggle_selected");
            var selectedIndexes = state?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList() ?? new List<int>();

            if (callbackData == null || selectedIndexes.Count == 0 && !callbackData.StartsWith("toggle_") && callbackData != "submit_toggle")
            {
                await UserStateTools.SetDataAsync(chatId.Value, "toggle_selected", string.Join(",", selectedIndexes));

                var keyboard = GenerateToggleKeyboard(selectedIndexes);
                await _botClient.SendTextMessageAsync(
                    chatId: chatId.Value,
                    text: "لطفاً دکمه‌های مورد نظر رو انتخاب کن:",
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken
                );
                return;
            }

            if (callbackData.StartsWith("toggle_"))
            {
                if (int.TryParse(callbackData.Split('_')[1], out int index))
                {
                    if (selectedIndexes.Contains(index))
                        selectedIndexes.Remove(index);
                    else
                        selectedIndexes.Add(index);

                    await UserStateTools.SetDataAsync(chatId.Value, "toggle_selected", string.Join(",", selectedIndexes));

                    var keyboard = GenerateToggleKeyboard(selectedIndexes);
                    await _botClient.EditMessageReplyMarkupAsync(
                        chatId: chatId.Value,
                        messageId: messageId.Value,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken
                    );
                }
                return;
            }

            if (callbackData == "submit_toggle")
            {
                var selected = selectedIndexes.Any()
                    ? $"شما این دکمه‌ها رو انتخاب کردید: {string.Join("، ", selectedIndexes.Select(i => (i + 1).ToString()))}"
                    : "شما هیچ گزینه‌ای انتخاب نکردید.";

                await _botClient.SendTextMessageAsync(
                    chatId: chatId.Value,
                    text: selected,
                    cancellationToken: cancellationToken
                );

                return;
            }
        }



        private InlineKeyboardMarkup GenerateToggleKeyboard(List<int> selectedIndexes)
        {
            var buttons = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < 8; i++)
            {
                string label = selectedIndexes.Contains(i) ? $"✅ {i + 1}" : (i + 1).ToString();
                var btn = InlineKeyboardButton.WithCallbackData(label, $"toggle_{i}");
                buttons.Add(new[] { btn });
            }

            var submitButton = InlineKeyboardButton.WithCallbackData("✅ ارسال انتخاب‌ها", "submit_toggle");
            buttons.Add(new[] { submitButton });

            return new InlineKeyboardMarkup(buttons);
        }


    }
}
