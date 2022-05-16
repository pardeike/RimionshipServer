using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RimionshipServer.Migrations
{
    public partial class TestUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Participants_ParticipantTwitchID",
                table: "Stats");

            migrationBuilder.RenameColumn(
                name: "ParticipantTwitchID",
                table: "Stats",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Stats_ParticipantTwitchID",
                table: "Stats",
                newName: "IX_Stats_ParticipantId");

            migrationBuilder.RenameColumn(
                name: "ModID",
                table: "Participants",
                newName: "Mod");

            migrationBuilder.RenameColumn(
                name: "TwitchID",
                table: "Participants",
                newName: "Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Stats",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Participants_ParticipantId",
                table: "Stats",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Participants_ParticipantId",
                table: "Stats");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "Stats",
                newName: "ParticipantTwitchID");

            migrationBuilder.RenameIndex(
                name: "IX_Stats_ParticipantId",
                table: "Stats",
                newName: "IX_Stats_ParticipantTwitchID");

            migrationBuilder.RenameColumn(
                name: "Mod",
                table: "Participants",
                newName: "ModID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Participants",
                newName: "TwitchID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Stats",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "getdate()");

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Participants_ParticipantTwitchID",
                table: "Stats",
                column: "ParticipantTwitchID",
                principalTable: "Participants",
                principalColumn: "TwitchID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
