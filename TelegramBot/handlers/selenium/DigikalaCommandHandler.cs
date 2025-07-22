using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.contract.Bot.selenium;
using TelegramBot.model.Enum;
using TelegramBot.Scraper;
using TelegramBot.Tool;

namespace TelegramBot.handlers.selenium
{
    public class DigikalaCommandHandler : IDigikalaHandler
    {
        private readonly TelegramBotClient _botClient;

        public DigikalaCommandHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task<bool> ShouldHandleAsync(Update update)
        {
            long? chatId = UserStateTools.GetChatId(update);

            if (chatId == null)
                return false;

            var step = await UserStateTools.GetStateAsync(chatId.Value);


            if (update.CallbackQuery != null && update.CallbackQuery.Data == "Selenium" ||
                await UserStateTools.GetMotherStateAsync(chatId) == nameof(UserBotState.InSelenium))
            {
                await UserStateTools.SetMotherAsync(chatId: chatId, nameof(UserBotState.InSelenium));
                return true;
            }
            return false;
        }
        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {
            await HandleProduct(update, cancellationToken);
        }

        public async Task HandleProduct(Update update, CancellationToken cancellationToken)
        {
            var chatid = UserStateTools.GetChatId(update);
            string query =  "لب تاپ";

            var scraper = new DigikalaScraper();
            var products = scraper.ScrapeProducts(query);

            if (products.Count == 0)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "متأسفانه محصولی پیدا نشد یا سایت دیجی‌کالا بارگذاری نشد.",
                    cancellationToken: cancellationToken
                );
                return;
            }

            var message = "🔍 نتایج جستجو:\n\n";
            foreach (var product in products)
            {
                message += $"📌 {product.Title}\n💰 {product.Price}\n\n";
            }

            if (message.Length > 4000)
                message = message.Substring(0, 4000) + "...\n(بخشی از متن حذف شد)";

            await _botClient.SendTextMessageAsync(
                chatId: chatid,
                text: message,
                cancellationToken: cancellationToken
            );
        }

    }
}
