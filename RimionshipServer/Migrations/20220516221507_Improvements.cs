using Microsoft.EntityFrameworkCore.Migrations;

namespace RimionshipServer.Migrations
{
	public partial class Improvements : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				 name: "IX_Participants_Twitch",
				 table: "Participants");

			migrationBuilder.RenameColumn(
				 name: "Twitch",
				 table: "Participants",
				 newName: "TwitchName");

			migrationBuilder.AddColumn<string>(
				 name: "TwitchId",
				 table: "Participants",
				 type: "TEXT",
				 nullable: true);

			migrationBuilder.CreateIndex(
				 name: "IX_Participants_TwitchId",
				 table: "Participants",
				 column: "TwitchId",
				 unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				 name: "IX_Participants_TwitchId",
				 table: "Participants");

			migrationBuilder.DropColumn(
				 name: "TwitchId",
				 table: "Participants");

			migrationBuilder.RenameColumn(
				 name: "TwitchName",
				 table: "Participants",
				 newName: "Twitch");

			migrationBuilder.CreateIndex(
				 name: "IX_Participants_Twitch",
				 table: "Participants",
				 column: "Twitch",
				 unique: true);
		}
	}
}
