namespace SudokuServer.Models.DatabaseModels.Context.OnSaveChangesActions;

public static class OnSaveChangesActionsExtensions
{
    public static IServiceCollection AddOnSaveChangesActions(this IServiceCollection services)
    {
        services.AddSingleton<UpdateTimeAction>();
        return services;
    }
}
