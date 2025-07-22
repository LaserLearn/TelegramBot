using Telegram.Bot.Types;
using TelegramBot.contract.Bot.start;
using TelegramBot.Services.user.menu;
using TelegramBot.Tool;

namespace TelegramBot.handlers.start
{
    public class StartHandler : IStartHandler
    {
        private readonly MenuService _menuService;

        public StartHandler(MenuService menuService)
        {
            _menuService = menuService;
        }

        public Task<bool> ShouldHandleAsync(Update update)
        {
            if (update.Message?.Text != "/start")
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {
            var messageText = update.Message?.Text;
            var chatid = UserStateTools.GetChatId(update);

            switch (messageText)
            {
                case "/start":
                    await UserStateTools.ClearAsync(chatid);
                    return HandleStartAsync(update, cancellationToken);

                default:
                    return Task.CompletedTask;
            }
        }



        public async Task HandleStartAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message?.Chat.Id;
            if (chatId == null) return;

            await _menuService.ShowMainMenuAsync(chatId.Value, cancellationToken);
        }
    }
}
