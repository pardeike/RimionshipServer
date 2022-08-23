using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class NewerStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountBloodCleaned",
                table: "LatestStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnimalMeatCreated",
                table: "LatestStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicksIgnoringBloodGod",
                table: "LatestStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicksLowColonistMood",
                table: "LatestStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmountBloodCleaned",
                table: "HistoryStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnimalMeatCreated",
                table: "HistoryStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicksIgnoringBloodGod",
                table: "HistoryStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicksLowColonistMood",
                table: "HistoryStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountBloodCleaned",
                table: "LatestStats");

            migrationBuilder.DropColumn(
                name: "AnimalMeatCreated",
                table: "LatestStats");

            migrationBuilder.DropColumn(
                name: "TicksIgnoringBloodGod",
                table: "LatestStats");

            migrationBuilder.DropColumn(
                name: "TicksLowColonistMood",
                table: "LatestStats");

            migrationBuilder.DropColumn(
                name: "AmountBloodCleaned",
                table: "HistoryStats");

            migrationBuilder.DropColumn(
                name: "AnimalMeatCreated",
                table: "HistoryStats");

            migrationBuilder.DropColumn(
                name: "TicksIgnoringBloodGod",
                table: "HistoryStats");

            migrationBuilder.DropColumn(
                name: "TicksLowColonistMood",
                table: "HistoryStats");
        }
    }
}
