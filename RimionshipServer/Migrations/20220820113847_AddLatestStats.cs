using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AddLatestStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "LatestStats",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    Wealth = table.Column<int>(type: "INTEGER", nullable: false),
                    MapCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Colonists = table.Column<int>(type: "INTEGER", nullable: false),
                    ColonistsNeedTending = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicalConditions = table.Column<int>(type: "INTEGER", nullable: false),
                    Enemies = table.Column<int>(type: "INTEGER", nullable: false),
                    WildAnimals = table.Column<int>(type: "INTEGER", nullable: false),
                    TamedAnimals = table.Column<int>(type: "INTEGER", nullable: false),
                    Visitors = table.Column<int>(type: "INTEGER", nullable: false),
                    Prisoners = table.Column<int>(type: "INTEGER", nullable: false),
                    DownedColonists = table.Column<int>(type: "INTEGER", nullable: false),
                    MentalColonists = table.Column<int>(type: "INTEGER", nullable: false),
                    Rooms = table.Column<int>(type: "INTEGER", nullable: false),
                    Caravans = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponDps = table.Column<int>(type: "INTEGER", nullable: false),
                    Electricity = table.Column<int>(type: "INTEGER", nullable: false),
                    Medicine = table.Column<int>(type: "INTEGER", nullable: false),
                    Food = table.Column<int>(type: "INTEGER", nullable: false),
                    Fire = table.Column<int>(type: "INTEGER", nullable: false),
                    Conditions = table.Column<int>(type: "INTEGER", nullable: false),
                    Temperature = table.Column<int>(type: "INTEGER", nullable: false),
                    NumRaidsEnemy = table.Column<int>(type: "INTEGER", nullable: false),
                    NumThreatBigs = table.Column<int>(type: "INTEGER", nullable: false),
                    ColonistsKilled = table.Column<int>(type: "INTEGER", nullable: false),
                    GreatestPopulation = table.Column<int>(type: "INTEGER", nullable: false),
                    InGameHours = table.Column<int>(type: "INTEGER", nullable: false),
                    DamageTakenPawns = table.Column<float>(type: "REAL", nullable: false),
                    DamageTakenThings = table.Column<float>(type: "REAL", nullable: false),
                    DamageDealt = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatestStats", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_LatestStats_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LatestStats");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
