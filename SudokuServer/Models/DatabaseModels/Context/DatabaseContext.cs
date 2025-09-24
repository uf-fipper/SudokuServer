using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SudokuServer.Models.DatabaseModels.Models;

namespace SudokuServer.Models.DatabaseModels.Context;

public class DatabaseContext : DbContext
{
    public DbSet<SudokuGame> SudokuGames { get; set; }

    public DatabaseContext() { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assem = Assembly.GetExecutingAssembly();
        var types = assem.GetTypes().Where(t => t.IsClass && !t.IsAbstract);
        foreach (var type in types)
        {
            var modelBuilderInterface = type.GetInterfaces()
                .FirstOrDefault(x => x == typeof(IDbModelBuilder));
            if (modelBuilderInterface == null)
                continue;
            var method = type.GetInterfaceMap(modelBuilderInterface)
                .TargetMethods.First(x => x.Name.EndsWith(nameof(IDbModelBuilder.OnModelCreating)));
            method.Invoke(null, [modelBuilder]);
        }
    }
}
