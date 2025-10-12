using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SudokuServer.Models.DatabaseModels.Models;

namespace SudokuServer.Models.DatabaseModels.Context;

public class DatabaseContext(
    DbContextOptions<DatabaseContext> options,
    IServiceProvider serviceProvider
) : DbContext(options)
{
    public DbSet<SudokuGame> SudokuGames { get; set; }

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

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        await OnSaveChangesAsync();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnSaveChanges()
    {
        foreach (var entityEntry in ChangeTracker.Entries())
        {
            foreach (var propertyEntry in entityEntry.Properties)
            {
                var property = propertyEntry.Metadata;
                // 找到这个property对应的实体类的属性
                var propInfo = entityEntry.Entity.GetType().GetProperty(property.Name);
                if (propInfo == null)
                    continue;
                // 判断这个属性是否有 OnSaveChangesAttribute 特性
                var onSaveChangesAttribute = propInfo.GetCustomAttribute<OnSaveChangesAttribute>();
                if (onSaveChangesAttribute is null)
                    continue;
                onSaveChangesAttribute.OnSaveChanges(serviceProvider, entityEntry, propertyEntry);
            }
        }
    }

    private async Task OnSaveChangesAsync()
    {
        foreach (var entityEntry in ChangeTracker.Entries())
        {
            foreach (var propertyEntry in entityEntry.Properties)
            {
                var property = propertyEntry.Metadata;
                // 找到这个property对应的实体类的属性
                var propInfo = entityEntry.Entity.GetType().GetProperty(property.Name);
                if (propInfo == null)
                    continue;
                // 判断这个属性是否有 OnSaveChangesAttribute 特性
                var onSaveChangesAttribute = propInfo.GetCustomAttribute<OnSaveChangesAttribute>();
                if (onSaveChangesAttribute is null)
                    continue;
                await onSaveChangesAttribute.OnSaveChangesAsync(
                    serviceProvider,
                    entityEntry,
                    propertyEntry
                );
            }
        }
    }
}
