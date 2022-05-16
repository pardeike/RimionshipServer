using Microsoft.EntityFrameworkCore.Migrations;

namespace RimionshipServer.Migrations
{
    public partial class AddIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ParticipantId",
                table: "Stats",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Twitch",
                table: "Participants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stats_Created",
                table: "Stats",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Mod",
                table: "Participants",
                column: "Mod");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Twitch",
                table: "Participants",
                column: "Twitch",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stats_Created",
                table: "Stats");

            migrationBuilder.DropIndex(
                name: "IX_Participants_Mod",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Participants_Twitch",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Twitch",
                table: "Participants");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "Stats",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Participants",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
