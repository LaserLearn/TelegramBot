using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using TelegramBot.model.Enum;
using TelegramBot.Tool;

namespace TelegramBot.Services.user.menu
{
    public class MenuService
    {
        private readonly TelegramBotClient _bot;

        public MenuService(TelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task ShowMainMenuAsync(long chatId, CancellationToken cancellationToken)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📢 ربات در گروه", MainMenuOption.Group.ToString()),
                InlineKeyboardButton.WithCallbackData("📺 ربات در کانال", MainMenuOption.Channel.ToString())
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📝 ربات در ثبت نام", MainMenuOption.Register.ToString()),
                InlineKeyboardButton.WithCallbackData("🤖 ربات در سلنیوم", MainMenuOption.Selenium.ToString())
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⏭ ربات در دکمه بعدی/قبلی", MainMenuOption.Paging.ToString())
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("✅ ربات در تیک و بک تیک", MainMenuOption.CheckMark.ToString())
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📂 ربات در منو تو در تو", MainMenuOption.NestedMenu.ToString())
            }
        });

            await _bot.SendTextMessageAsync(
                chatId,
                "لطفاً یکی از گزینه‌های زیر را انتخاب کن:",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );

            await UserStateTools.SetStateAsync(chatId, UserBotState.Menu.ToString());
        }
    }

}
