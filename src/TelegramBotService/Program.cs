using StackExchange.Redis;
using Telegram.Bot;
using TelegramBotService;
using TelegramBotService.Handlers;
using TelegramBotService.HttpClients;
using TelegramBotService.Session;

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;

builder.Services.AddSingleton<ITelegramBotClient>(
    new TelegramBotClient(config["Telegram:Token"]!));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(config["Redis:Connection"]!));

builder.Services.AddSingleton<SessionService>();

var gatewayUrl = config["ApiGateway:BaseUrl"]!;

builder.Services.AddHttpClient<PatientApiClient>(c => c.BaseAddress = new Uri(gatewayUrl));
builder.Services.AddHttpClient<ClinicApiClient>(c => c.BaseAddress = new Uri(gatewayUrl));
builder.Services.AddHttpClient<BookingApiClient>(c => c.BaseAddress = new Uri(gatewayUrl));

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
