using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SudokuServer.Models.DatabaseModels.Context;

public interface IDbModelBuilder
{
    static abstract void OnModelCreating(ModelBuilder modelBuilder);
}

public interface IDbModelBuilder<T> : IDbModelBuilder
    where T : class, IDbModelBuilder<T>
{
    static void IDbModelBuilder.OnModelCreating(ModelBuilder modelBuilder) =>
        T.OnModelCreating(modelBuilder.Entity<T>());

    static abstract void OnModelCreating(EntityTypeBuilder<T> builder);
}
