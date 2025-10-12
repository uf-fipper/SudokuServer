using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WinBoard",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏获胜时的版块",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏获胜时的版块")
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdateTime",
                table: "SudokuGames",
                type: "datetimeoffset",
                nullable: false,
                comment: "数独游戏更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "数独游戏更新时间")
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "SudokuGames",
                type: "int",
                nullable: false,
                comment: "数独游戏类型",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数独游戏类型")
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "StartBoard",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏开始时的版块",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏开始时的版块")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "SudokuGames",
                type: "int",
                nullable: false,
                comment: "数独游戏版块大小",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数独游戏版块大小")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<int>(
                name: "Seed",
                table: "SudokuGames",
                type: "int",
                nullable: false,
                comment: "数独游戏随机种子",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数独游戏随机种子")
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreateTime",
                table: "SudokuGames",
                type: "datetimeoffset",
                nullable: false,
                comment: "数独游戏创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "数独游戏创建时间")
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "Board",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏版块",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏版块")
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SudokuGames",
                type: "uniqueidentifier",
                maxLength: 36,
                nullable: false,
                comment: "数独游戏id",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 36,
                oldComment: "数独游戏id")
                .Annotation("Relational:ColumnOrder", 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WinBoard",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏获胜时的版块",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏获胜时的版块")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateTime",
                table: "SudokuGames",
                type: "datetime2",
                nullable: false,
                comment: "数独游戏更新时间",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldComment: "数独游戏更新时间")
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "SudokuGames",
                type: "int",
                nullable: false,
                comment: "数独游戏类型",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数独游戏类型")
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "StartBoard",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏开始时的版块",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏开始时的版块")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "SudokuGames",
                type: "int",
                nullable: false,
                comment: "数独游戏版块大小",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数独游戏版块大小")
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<int>(
                name: "Seed",
                table: "SudokuGames",
                type: "int",
                nullable: false,
                comment: "数独游戏随机种子",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数独游戏随机种子")
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "SudokuGames",
                type: "datetime2",
                nullable: false,
                comment: "数独游戏创建时间",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldComment: "数独游戏创建时间")
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "Board",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏版块",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏版块")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SudokuGames",
                type: "uniqueidentifier",
                maxLength: 36,
                nullable: false,
                comment: "数独游戏id",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 36,
                oldComment: "数独游戏id")
                .OldAnnotation("Relational:ColumnOrder", 0);
        }
    }
}
