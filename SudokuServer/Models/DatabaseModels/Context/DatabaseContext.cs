using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace SudokuServer.Models.DatabaseModels.Context;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assem = Assembly.GetExecutingAssembly();
        var types = assem.GetTypes();
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
