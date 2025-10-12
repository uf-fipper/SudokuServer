using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SudokuServer.Models.DatabaseModels.Context.OnSaveChangesActions;

public class UpdateTimeAction : IOnSaveChangesAction
{
    public void OnSaveChanges(EntityEntry entry, PropertyEntry property)
    {
        property.CurrentValue = DateTime.Now;
    }
}
