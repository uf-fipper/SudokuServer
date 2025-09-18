using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SudokuServer.Models.DatabaseModels.Context;

public interface IDbModel<T>
    where T : class, IDbModel<T>
{
    public static void OnModelCreating(ModelBuilder modelBuilder) =>
        T.OnModelCreating(modelBuilder.Entity<T>());

    public static abstract void OnModelCreating(EntityTypeBuilder<T> builder);
}
