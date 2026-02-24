using System.Text.Json.Serialization;
using EventStore.Client;
using FGS.Adapters.ConnectionTracker;
using FGS.Adapters.JsonConvert.Lobby;
using FGS.App;
using FGS.DAL.BackgroundServices;
using FGS.DAL.EventSourceRepositories;
using FGS.DAL.ViewModel;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Events;
using FGS.Domain.Services;
using FGS.Host.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAuthContext();

builder.Services.ConfigureApplicationCookie(options =>
{   
    options.Cookie.HttpOnly = true; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// event store db

var settings = EventStoreClientSettings.Create(builder.Configuration.GetConnectionString("EventStoreConnection")!);
builder.Services.AddSingleton(new EventStoreClient(settings));
builder.Services.AddScoped<IAggregateRepository<Lobby, LobbyEvent>, LobbyRepository>();

builder.Services.AddHostedService<EventStoreBackgroundService>();

// app
builder.Services.AddScoped<LobbyAppService>();
builder.Services.AddSingleton<ILobbyEventJsonConvert, LobbyEventJsonConvert>();
builder.Services.AddScoped<IFgsViewModelRepository, FgsViewModelRepository>();

// view model
builder.AddViewModelContext();

// adapters
builder.Services.AddScoped<IConnectionTrackerService, ConnectionTrackerService>();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await Task.WhenAll(
    app.Services.AuthConfigureAsync(),
    app.Services.ViewModelConfigureAsync());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program { }
