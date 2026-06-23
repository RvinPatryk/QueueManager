using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Opis = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Priorytet = table.Column<int>(type: "INTEGER", nullable: false),
                    Autor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OsobaPrzypisana = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataUtworzenia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataRozpoczecia = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataUkonczenia = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PrzewidzianyCzas = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Termin = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
