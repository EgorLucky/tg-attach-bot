using DomainLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.CommandRouting;
using WebHookApp;
using WebHookApp.AuthenticationTelegram;
using WebHookApp.AuthorizationPolicies.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var botConfig = new BotConfiguration
{
    BotToken = builder.Configuration.GetValue<string>("botToken"),
    HostAddress = builder.Configuration.GetValue<string>("webAppHost"),
    BotUsername = builder.Configuration.GetValue<string>("botUsername"),
};

var useWebhook = builder.Configuration.GetValue("useWebhook", true);

builder.Services.AddSingleton(botConfig);

if(useWebhook)
    builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.BotToken, httpClient));
builder.Services
    .AddScoped<HandleUpdateService>()
    .AddScoped<TelegramBotCommandRouter>()
    .AddScoped<TelegramBotController, TelegramController>()
    .AddScoped<IAuthorizationChatHandler, AuthorizationChatHandler>();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp", Version = "v1" });
});
builder.Services
    .AddTransient<TelegramDomainService>()
    .AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetValue<string>("dbConnectionString")))
    .AddHttpClient();

builder.Services.AddAuthorization(
                options =>
                {

                    options.AddPolicy("RegistredUser", policy =>
                        policy.Requirements.Add(new RegistredUserRequirement(true)));

                    options.AddPolicy("TelegramRegistredUser", policy =>
                        policy.Requirements.Add(new RegistredTelegramUserRequirement(true)));

                    options.AddPolicy("TelegramValidUser", policy =>
                        policy.Requirements.Add(new ValidTelegramUserRequirement(true)));
                })
                .AddAuthentication("Bearer")
                .AddTelegramWebAppScheme("Bearer");
builder.Services.AddTransient<TelegramAuthenticationHandler>();

builder.Services
    .AddScoped<IAuthorizationHandler, TelegramValidUserHandler>();

builder.Services.AddCors(options =>
                options.AddPolicy("CorsPolicy", builder =>
                    builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}
app.UseCors("CorsPolicy");
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1");
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
