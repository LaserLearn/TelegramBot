using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.contract.Bot.NextPast;
using TelegramBot.model.Enum;
using TelegramBot.model.user;
using TelegramBot.Tool;

namespace TelegramBot.handlers.NextPast
{
    public class NextPastCommandHandler : INextPastHandlers
    {
        private readonly TelegramBotClient _botClient;

        public NextPastCommandHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<bool> ShouldHandleAsync(Update update)
        {
            long? chatId = UserStateTools.GetChatId(update);

            if (chatId == null)
                return false;

            var step = await UserStateTools.GetStateAsync(chatId.Value);


            if (update.CallbackQuery != null && update.CallbackQuery.Data == "Paging" ||
                await UserStateTools.GetMotherStateAsync(chatId) == nameof(UserBotState.InPaging))
            {
                await UserStateTools.SetMotherAsync(chatId: chatId, nameof(UserBotState.InPaging));
                return true;
            }
            return false;
        }
        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {
            long? chatId = UserStateTools.GetChatId(update);

            var step = await UserStateTools.GetStateAsync(chatId.Value);

            if (update.CallbackQuery?.Data != null && update.CallbackQuery.Data.StartsWith("userpage_"))
            {
                await HandleUserPageAsync(update, cancellationToken);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Data != null)
            {
                switch (update.CallbackQuery.Data)
                {
                    default:
                        await HandleUserPageAsync(update, cancellationToken);
                        return;
                }
            }
            switch (step)
            {
                default:
                    await HandleUserPageAsync(update, cancellationToken);
                    return;
            }
        }

        public async Task HandleUserPageAsync(Update update, CancellationToken cancellationToken)
        {
            var callbackData = update.CallbackQuery!.Data;
            var chatId = UserStateTools.GetChatId(update);

            int index = 0;
            try
            {
                 index = int.Parse(callbackData!.Split('_')[1]);
            }
            catch
            {
                 index = 0;
            }

            var users = new List<User_E>()
            {
                new User_E(){ age = 21 , email = "kasramasra911@gmail.com", role_Em = Role_Em.Admin , user_name = "kasra"},
                new User_E(){ age = 100 , email = "LaserLearn.dev@gmail.com", role_Em = Role_Em.Admin , user_name = "LaserLearn"},
                new User_E(){ age = 2 , email = "SARAPPOOPPOOPPOO@gmail.com", role_Em = Role_Em.user , user_name = "sara"},
            };
            if (index < 0 || index >= users.Count)
                return;

            var user = users[index];

            var messageText = $"👤 نام: {user.user_name}\n📧 ایمیل: {user.email}\n نقش: {nameof(user.role_Em)}";

            var buttons = new List<InlineKeyboardButton[]>();

            if (index > 0)
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("⬅️ قبلی", $"userpage_{index - 1}") });

            if (index < users.Count - 1)
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("➡️ بعدی", $"userpage_{index + 1}") });

            var keyboard = new InlineKeyboardMarkup(buttons);

            await _botClient.EditMessageTextAsync(
                chatId: chatId!,
                messageId: update!.CallbackQuery!.Message!.MessageId,
                text: messageText,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }

    }
}
