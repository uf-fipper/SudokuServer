using Microsoft.EntityFrameworkCore;
using SudokuServer.Models.DatabaseModels.Context;
using SudokuServer.Services;
using SudokuServer.ServicesImpl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// database
var databaseConnectionString =
    Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DatabaseConnection")
    ?? throw new InvalidOperationException(
        "Environment variable 'DATABASE_CONNECTION_STRING' not found."
    );
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(databaseConnectionString)
);

// services
builder.Services.AddScoped<ISudokuService, SudokuService>();

// lock
builder.Services.AddSingleton<IDistributedLock, RedisDistributedLock>();

// cache
builder.Services.AddSingleton<IDistributedCacheMore, RedisDistributedCache>();

// redis
var redisOptions = new StackExchange.Redis.ConfigurationOptions
{
    EndPoints = { Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost:6379" },
    User = builder.Configuration.GetValue<string>("REDIS_USER"),
    Password = builder.Configuration.GetValue<string>("REDIS_PASSWORD"),
    DefaultDatabase = builder.Configuration.GetValue<int?>("REDIS_DATABASE") ?? 0,
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

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
