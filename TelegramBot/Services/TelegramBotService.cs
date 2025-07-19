using Telegram.Bot;
using TelegramBot.contract.Bot;
namespace TelegramBot.Services
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private TelegramBotClient? _botClient;

        public TelegramBotService(
            ILogger<TelegramBotService> logger,
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var token = _configuration["TelegramBotToken"];
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Bot token is missing.");
                return;
            }

            _botClient = new TelegramBotClient(token);

            var me = await _botClient.GetMeAsync();
            _logger.LogInformation($"Bot started: {me.Username}");

            _botClient.StartReceiving(
                async (botClient, update, token) =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var handlers = scope.ServiceProvider.GetServices<ITelegramUpdateHandler>();

                        foreach (var handler in handlers)
                        {
                            if (await handler.ShouldHandleAsync(update))
                            {
                                await handler.DispatchAsync(update, token);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while handling update.");
                    }
                },
                HandleErrorAsync,
                cancellationToken: stoppingToken
            );

            await Task.Delay(-1, stoppingToken);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Telegram bot error");
            return Task.CompletedTask;
        }
    }


}
