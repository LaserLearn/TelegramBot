using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.model.Enum;
using TelegramBot.Tool;

namespace TelegramBot.Services.channel
{
    public class ChannelService
    {
        private readonly TelegramBotClient _bot;

        public ChannelService(TelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task ShowMainMenuAsync(long chatId, CancellationToken cancellationToken)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("ارسال پیام به کانال", nameof(ChannelState.SendMessage)),
                InlineKeyboardButton.WithCallbackData("ارسال عکس به کانال", nameof(ChannelState.SendImage))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("ارسال فیلم به کانال", nameof(ChannelState.SendFilm)),
                InlineKeyboardButton.WithCallbackData("اسال نظرسنجی به کانال", nameof(ChannelState.SendPoll))
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("بازگشت", nameof(ChannelState.None)),
            }
        });

            await _bot.SendTextMessageAsync(
                chatId,
                "لطفاً یکی از گزینه‌های زیر را انتخاب کن:",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );

            await UserStateTools.SetStateAsync(chatId, ChannelState.ChannelMenu.ToString());
        }
    }
}
