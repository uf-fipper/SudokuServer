using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateTimeAndAddWinBoard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateTime",
                table: "SudokuGames",
                type: "datetime2",
                nullable: false,
                comment: "数独游戏更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "getdate()",
                oldComment: "数独游戏更新时间");

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
                oldComment: "数独游戏开始时的版块，结构为json");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "SudokuGames",
                type: "datetime2",
                nullable: false,
                comment: "数独游戏创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "getdate()",
                oldComment: "数独游戏创建时间");

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
                oldComment: "数独游戏版块，结构为json");

            migrationBuilder.AddColumn<string>(
                name: "WinBoard",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                comment: "数独游戏获胜时的版块");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinBoard",
                table: "SudokuGames");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateTime",
                table: "SudokuGames",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getdate()",
                comment: "数独游戏更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "数独游戏更新时间");

            migrationBuilder.AlterColumn<string>(
                name: "StartBoard",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏开始时的版块，结构为json",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏开始时的版块");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "SudokuGames",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getdate()",
                comment: "数独游戏创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "数独游戏创建时间");

            migrationBuilder.AlterColumn<string>(
                name: "Board",
                table: "SudokuGames",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "数独游戏版块，结构为json",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "数独游戏版块");
        }
    }
}
