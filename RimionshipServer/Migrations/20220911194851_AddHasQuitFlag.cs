using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AddHasQuitFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasQuit",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasQuit",
                table: "AspNetUsers");
        }
    }
}
