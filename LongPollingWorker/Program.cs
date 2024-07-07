using Telegram.Bot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DomainLogic;
using Telegram.Bot.CommandRouting;


AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
{
    Console.WriteLine(e.ExceptionObject);
};

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        var bot = new TelegramBotClient(configuration.GetValue<string>("botToken"));
        
        var botConfig = new BotConfiguration
        {
            BotToken = configuration.GetValue<string>("botToken"),
            HostAddress = configuration.GetValue<string>("webAppHost"),
            BotUsername = configuration.GetValue<string>("botUsername"),
        };
        services.AddSingleton(botConfig);

        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.BotToken, httpClient));
        services
            //.AddScoped<HandleUpdateService>()
            .AddScoped<TelegramBotCommandRouter>()
            .AddScoped<TelegramBotController, TelegramController>()
            .AddScoped<IAuthorizationChatHandler, AuthorizationChatHandler>();

        services.AddSingleton<ITelegramBotClient>(bot);
        services.AddTransient<TelegramController>();
        services.AddTransient<TelegramDomainService>();
        services.AddHttpClient();
        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(configuration.GetValue<string>("dbConnectionString"), 
            npgSql => npgSql.MigrationsAssembly(typeof(Program).Assembly.FullName)));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
