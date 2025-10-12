using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SudokuServer.Models.DatabaseModels.Context;

public abstract class OnSaveChangesAttribute : Attribute
{
    public abstract void OnSaveChanges(
        IServiceProvider serviceProvider,
        EntityEntry entry,
        PropertyEntry property
    );

    public abstract Task OnSaveChangesAsync(
        IServiceProvider serviceProvider,
        EntityEntry entry,
        PropertyEntry property
    );
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class OnSaveChangesAttribute<T> : OnSaveChangesAttribute
    where T : IOnSaveChangesAction
{
    public override void OnSaveChanges(
        IServiceProvider serviceProvider,
        EntityEntry entry,
        PropertyEntry property
    )
    {
        var t = serviceProvider.GetRequiredService<T>();
        t.OnSaveChanges(entry, property);
    }

    public override Task OnSaveChangesAsync(
        IServiceProvider serviceProvider,
        EntityEntry entry,
        PropertyEntry property
    )
    {
        var t = serviceProvider.GetRequiredService<T>();
        return t.OnSaveChangesAsync(entry, property);
    }
}

public interface IOnSaveChangesAction
{
    void OnSaveChanges(EntityEntry entry, PropertyEntry property);

    Task OnSaveChangesAsync(EntityEntry entry, PropertyEntry property)
    {
        OnSaveChanges(entry, property);
        return Task.CompletedTask;
    }
}
