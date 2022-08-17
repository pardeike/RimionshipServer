using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AllowedMods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllowedMods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackageId = table.Column<string>(type: "TEXT", nullable: false),
                    SteamId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedMods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllowedMods_PackageId",
                table: "AllowedMods",
                column: "PackageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllowedMods_SteamId",
                table: "AllowedMods",
                column: "SteamId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowedMods");
        }
    }
}
