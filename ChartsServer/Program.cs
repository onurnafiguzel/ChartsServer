using ChartsServer.Models;
using ChartsServer.Subscription;
using ChartsServer.Subscription.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DatabaseSubscription<Satislar>>();
builder.Services.AddSingleton<DatabaseSubscription<Personeller>>();

var app = builder.Build();

app.UseDatabaseSubscription<DatabaseSubscription<Satislar>>("Satislar");
app.UseDatabaseSubscription<DatabaseSubscription<Personeller>>("Personeller");

app.MapGet("/", () => "Hello World!");

app.Run();
