using MongoDB.Driver;
using Telegram.Bot;
using TelegramBot.config.mongo;
using TelegramBot.contract.Bot;
using TelegramBot.contract.Database.comment;
using TelegramBot.contract.Database.generic;
using TelegramBot.contract.Database.user;
using TelegramBot.handlers.channel;
using TelegramBot.handlers.group;
using TelegramBot.handlers.NextPast;
using TelegramBot.handlers.register;
using TelegramBot.handlers.selenium;
using TelegramBot.handlers.start;
using TelegramBot.handlers.Toggle;
using TelegramBot.Imp.Database.comment;
using TelegramBot.Imp.Database.generic;
using TelegramBot.Imp.Database.user;
using TelegramBot.Services;
using TelegramBot.Services.channel;
using TelegramBot.Services.user.menu;
using TelegramBot.Tool;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region mongo
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

var mongoSettings = new MongoDBSettings();
builder.Configuration.GetSection("MongoDBSettings").Bind(mongoSettings);

builder.Services.AddSingleton(mongoSettings);

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(mongoSettings.Develop_DataBase.ConnectionString));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.Develop_DataBase.DatabaseName);
});


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IComment_rep, Comment_rep>();
builder.Services.AddScoped<IUser_Rep, User_Rep>();


#endregion

#region Bot
// ثبت کانفیگ ربات
builder.Services.AddSingleton<TelegramBotClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var token = config["TelegramBotToken"];
    return new TelegramBotClient(token);
});

// ثبت سایر سرویس‌ها
builder.Services.AddHostedService<TelegramBotService>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<ChannelService>();

builder.Services.AddScoped<ITelegramUpdateHandler, StartHandler>();
builder.Services.AddScoped<ITelegramUpdateHandler, RegisterCommandHandler>();
builder.Services.AddScoped<ITelegramUpdateHandler, ChannelCommandHandler>();
builder.Services.AddScoped<ITelegramUpdateHandler, NextPastCommandHandler>();
builder.Services.AddScoped<ITelegramUpdateHandler, ToggleSelectionHandler>();
builder.Services.AddScoped<ITelegramUpdateHandler, GroupCommandHandler>();
builder.Services.AddScoped<ITelegramUpdateHandler, DigikalaCommandHandler>();


#endregion


var app = builder.Build();

#region mongo
var db = app.Services.GetRequiredService<IMongoDatabase>();
UserStateTools.Initialize(db);
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
