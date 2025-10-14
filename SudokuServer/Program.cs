using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SudokuServer.Models.DatabaseModels.Context;
using SudokuServer.Models.DatabaseModels.Context.OnSaveChangesActions;
using SudokuServer.Services;
using SudokuServer.ServicesImpl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// database
var databaseOptions =
    builder.Configuration.GetSection("Database")
    ?? throw new InvalidOperationException("Database section not found.");
var databaseType =
    databaseOptions.GetValue<string>("Type")
    ?? throw new InvalidOperationException("Database type not found.");
var databaseConnectionString =
    databaseOptions.GetValue<string>("ConnectionString")
    ?? throw new InvalidOperationException("Database connection string not found.");
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    switch (databaseType)
    {
        case "SqlServer":
            options.UseSqlServer(databaseConnectionString);
            break;
        case "MySql":
            options.UseMySQL(databaseConnectionString);
            break;
        default:
            throw new InvalidOperationException($"Database type {databaseType} not supported.");
    }
});

// services
builder.Services.AddScoped<SudokuService>();
builder.Services.AddScoped<GamesManager>();
builder.Services.AddOnSaveChangesActions();

// lock
builder.Services.AddSingleton<IDistributedLock, RedisDistributedLock>();

// cache
builder.Services.AddSingleton<IDistributedCacheMore, RedisDistributedCache>();

// redis
var redisSection = builder.Configuration.GetSection("Redis");
var redisEndpoint = redisSection.GetValue<string>("Endpoint") ?? "localhost:6379";

// throw new Exception(redisEndpoint);
var redisOptions = new StackExchange.Redis.ConfigurationOptions
{
    EndPoints = { redisEndpoint },
    User = redisSection.GetValue<string>("User"),
    Password = redisSection.GetValue<string>("Password"),
    DefaultDatabase = redisSection.GetValue<int?>("Database") ?? 0,
};
var redisConnection = StackExchange.Redis.ConnectionMultiplexer.Connect(redisOptions);
builder.Services.AddSingleton(redisConnection.GetDatabase(0));

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// websockets
app.UseWebSockets();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
