using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using SudokuServer.Models.DatabaseModels.Context;

namespace SudokuServer.Models.DatabaseModels.Models;

public partial class SudokuGame : IDbModelBuilder<SudokuGame>
{
    public Guid Id { get; set; }

    public string StartBoard { get; set; } = null!;

    public string Board { get; set; } = null!;

    public int Seed { get; set; }

    public int Size { get; set; }

    public SudokuGameType Type { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<SudokuGame> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasMaxLength(36).HasComment("数独游戏id").ValueGeneratedOnAdd();
        entity
            .Property(e => e.StartBoard)
            .HasMaxLength(255)
            .HasComment("数独游戏开始时的版块，结构为json");
        entity.Property(e => e.Board).HasMaxLength(255).HasComment("数独游戏版块，结构为json");
        entity.Property(e => e.Seed).HasComment("数独游戏随机种子");
        entity.Property(e => e.Size).HasComment("数独游戏版块大小");
        entity.Property(e => e.Type).HasComment("数独游戏类型");
        entity
            .Property(e => e.CreateTime)
            .HasComment("数独游戏创建时间")
            // .ValueGeneratedOnAdd()
            .HasDefaultValueSql("getdate()"); // sqlserver 只能这样写
        entity
            .Property(e => e.UpdateTime)
            .HasComment("数独游戏更新时间")
            // .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("getdate()"); // sqlserver 只能这样写
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
