using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FutureEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParticipantId = table.Column<long>(type: "INTEGER", nullable: false),
                    Ticks = table.Column<int>(type: "INTEGER", nullable: false),
                    ColonyWealth = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Faction = table.Column<string>(type: "TEXT", nullable: true),
                    Points = table.Column<float>(type: "REAL", nullable: false),
                    Strategy = table.Column<string>(type: "TEXT", nullable: true),
                    ArrivalMode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TwitchId = table.Column<string>(type: "TEXT", nullable: true),
                    TwitchName = table.Column<string>(type: "TEXT", nullable: true),
                    Mod = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParticipantId = table.Column<long>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "getdate()"),
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
                    table.PrimaryKey("PK_Stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stats_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FutureEvents_ParticipantId",
                table: "FutureEvents",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FutureEvents_Ticks",
                table: "FutureEvents",
                column: "Ticks");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Mod",
                table: "Participants",
                column: "Mod");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_TwitchId",
                table: "Participants",
                column: "TwitchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stats_Created",
                table: "Stats",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_ParticipantId",
                table: "Stats",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureEvents");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Participants");
        }
    }
}
