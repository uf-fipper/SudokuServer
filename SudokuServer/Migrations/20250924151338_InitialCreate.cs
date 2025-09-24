using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SudokuGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false, comment: "数独游戏id"),
                    StartBoard = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "数独游戏开始时的版块，结构为json"),
                    Board = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "数独游戏版块，结构为json"),
                    Seed = table.Column<int>(type: "int", nullable: false, comment: "数独游戏随机种子"),
                    Size = table.Column<int>(type: "int", nullable: false, comment: "数独游戏版块大小"),
                    Type = table.Column<int>(type: "int", nullable: false, comment: "数独游戏类型"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "数独游戏创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "数独游戏更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SudokuGames", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SudokuGames");
        }
    }
}
