using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace SudokuServer.Models.DatabaseModels.Context;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
}
