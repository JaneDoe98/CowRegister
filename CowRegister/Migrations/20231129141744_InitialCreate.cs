using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CowRegister.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cattle",
                columns: table => new
                {
                    Enar = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EarLetter = table.Column<int>(type: "INTEGER", nullable: false),
                    MotherEnar = table.Column<int>(type: "INTEGER", nullable: false),
                    Birth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    Breed = table.Column<int>(type: "INTEGER", nullable: false),
                    ArrivedStock = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AbandonedStock = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoteOfAbandon = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cattle", x => x.Enar);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cattle");
        }
    }
}
