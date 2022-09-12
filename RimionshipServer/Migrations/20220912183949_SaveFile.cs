using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class SaveFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaveFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    File = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MD5 = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaveFiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaveFiles");
        }
    }
}
