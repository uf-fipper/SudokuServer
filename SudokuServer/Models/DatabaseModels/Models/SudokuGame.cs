using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using SudokuServer.Models.DatabaseModels.Context;
using SudokuServer.Models.DatabaseModels.Context.OnSaveChangesActions;

namespace SudokuServer.Models.DatabaseModels.Models;

public partial class SudokuGame : IDbModelBuilder<SudokuGame>
{
    public Guid Id { get; set; }

    public string StartBoard { get; set; } = null!;

    public string Board { get; set; } = null!;

    public string WinBoard { get; set; } = null!;

    public int Seed { get; set; }

    public int Size { get; set; }

    public SudokuGameType Type { get; set; }

    public DateTime CreateTime { get; set; }

    [OnSaveChanges<UpdateTimeAction>]
    public DateTime UpdateTime { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<SudokuGame> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasMaxLength(36).HasComment("数独游戏id").ValueGeneratedOnAdd();
        entity.Property(e => e.StartBoard).HasMaxLength(255).HasComment("数独游戏开始时的版块");
        entity.Property(e => e.Board).HasMaxLength(255).HasComment("数独游戏版块");
        entity.Property(e => e.WinBoard).HasMaxLength(255).HasComment("数独游戏获胜时的版块");
        entity.Property(e => e.Seed).HasComment("数独游戏随机种子");
        entity.Property(e => e.Size).HasComment("数独游戏版块大小");
        entity.Property(e => e.Type).HasComment("数独游戏类型");
        entity
            .Property(e => e.CreateTime)
            .HasComment("数独游戏创建时间")
            .ValueGeneratedOnAdd()
            .HasValueGenerator<DateTimeNowGenerator>();
        // efcore 不支持在更新时自动更新字段，只能在代码里手动更新
        // https://github.com/dotnet/efcore/issues/6999
        entity
            .Property(e => e.UpdateTime)
            .HasComment("数独游戏最后更新时间")
            // .ValueGeneratedOnAddOrUpdate()
            // .HasValueGenerator<DateTimeOffsetNowGenerator>()
        ;
    }
}

public enum SudokuGameType
{
    Default = 0,
}

public class DateTimeNowGenerator : ValueGenerator<DateTime>
{
    public override bool GeneratesTemporaryValues => false;

    public override DateTime Next(EntityEntry entry)
    {
        return DateTime.Now;
    }
}
