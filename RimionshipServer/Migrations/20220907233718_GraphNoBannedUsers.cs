using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class GraphNoBannedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountUser",
                table: "GraphData",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoryStats_Timestamp",
                table: "HistoryStats",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HistoryStats_Timestamp",
                table: "HistoryStats");

            migrationBuilder.DropColumn(
                name: "CountUser",
                table: "GraphData");
        }
    }
}
