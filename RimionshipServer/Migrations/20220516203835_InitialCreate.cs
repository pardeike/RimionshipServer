using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RimionshipServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    TwitchID = table.Column<string>(type: "TEXT", nullable: false),
                    ModID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.TwitchID);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ColonyWealth = table.Column<long>(type: "INTEGER", nullable: false),
                    ParticipantTwitchID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stats_Participants_ParticipantTwitchID",
                        column: x => x.ParticipantTwitchID,
                        principalTable: "Participants",
                        principalColumn: "TwitchID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stats_ParticipantTwitchID",
                table: "Stats",
                column: "ParticipantTwitchID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Participants");
        }
    }
}
