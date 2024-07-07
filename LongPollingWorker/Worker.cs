using DomainLogic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.CommandRouting;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _provider;
    private readonly HttpClient _client;
    private readonly ITelegramBotClient _bot;

    public Worker(ILogger<Worker> logger, IServiceProvider provider, HttpClient client, ITelegramBotClient bot)
    {
        _logger = logger;
        _provider = provider;
        _client = client;
        _bot = bot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        _bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(10000000, stoppingToken);
            try
            {
                var response = _client.GetAsync("https://egorluckydevdomain.ru");
            }
            catch(Exception e)
            {
                _logger.LogError("FAILED TO REQUEST TO https://egorluckydevdomain.ru");
                _logger.LogError(e.ToString());
            }
        }
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = default(long);
        try
        {
            var id = Guid.NewGuid();
            _logger.LogInformation($"received message {id} {DateTimeOffset.Now}");
            chatId = update.MyChatMember?.Chat?.Id ??
                            update?.Message?.Chat?.Id ??
                            0;

            using var scope = _provider.CreateScope();
            var controllers = scope.ServiceProvider.GetService<IEnumerable<TelegramBotController>>();

            //var controllers = new List<TelegramBotController>() { controller };
            var router = new TelegramBotCommandRouter(
                controllers,
                botClient,
                new AuthorizationChatHandler(controllers));

            await router.TryRunCommand(update);

            _logger.LogInformation($"processed message {id} {DateTimeOffset.Now}");
        }
        catch(Exception ex)
        {
            _logger.LogInformation(ex.ToString());
            try
            {
                await botClient.SendTextMessageAsync(chatId, ex.ToString());
            }
            catch (Exception ex2)
            {
                _logger.LogInformation(ex2.ToString());
            }
        }
    }

    async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Некоторые действия
        _logger.LogInformation(exception.ToString());
    }
}
