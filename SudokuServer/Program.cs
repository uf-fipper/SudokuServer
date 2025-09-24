using Microsoft.EntityFrameworkCore;
using SudokuServer.Models.DatabaseModels.Context;
using SudokuServer.Services;
using SudokuServer.ServicesImpl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString =
    builder.Configuration.GetConnectionString("DatabaseConnection")
    ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.");

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

// services
builder.Services.AddScoped<ISudokuService, SudokuService>();

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
