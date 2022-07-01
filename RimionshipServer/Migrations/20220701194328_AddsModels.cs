using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AddsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Participants_ParticipantId",
                table: "Stats");

            migrationBuilder.RenameColumn(
                name: "ColonyWealth",
                table: "Stats",
                newName: "WildAnimals");

            migrationBuilder.AlterColumn<long>(
                name: "ParticipantId",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Caravans",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Colonists",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColonistsKilled",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColonistsNeedTending",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Conditions",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "DamageDealt",
                table: "Stats",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DamageTakenPawns",
                table: "Stats",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DamageTakenThings",
                table: "Stats",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "DownedColonists",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Electricity",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Enemies",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Fire",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Food",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GreatestPopulation",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InGameHours",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MapCount",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MedicalConditions",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Medicine",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MentalColonists",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumRaidsEnemy",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumThreatBigs",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Prisoners",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rooms",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TamedAnimals",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Temperature",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Visitors",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wealth",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeaponDps",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_FutureEvents_ParticipantId",
                table: "FutureEvents",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FutureEvents_Ticks",
                table: "FutureEvents",
                column: "Ticks");

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Participants_ParticipantId",
                table: "Stats",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Participants_ParticipantId",
                table: "Stats");

            migrationBuilder.DropTable(
                name: "FutureEvents");

            migrationBuilder.DropColumn(
                name: "Caravans",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Colonists",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "ColonistsKilled",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "ColonistsNeedTending",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Conditions",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "DamageDealt",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "DamageTakenPawns",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "DamageTakenThings",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "DownedColonists",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Electricity",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Enemies",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Fire",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Food",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "GreatestPopulation",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "InGameHours",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "MapCount",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "MedicalConditions",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Medicine",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "MentalColonists",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "NumRaidsEnemy",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "NumThreatBigs",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Prisoners",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Rooms",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "TamedAnimals",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Visitors",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Wealth",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "WeaponDps",
                table: "Stats");

            migrationBuilder.RenameColumn(
                name: "WildAnimals",
                table: "Stats",
                newName: "ColonyWealth");

            migrationBuilder.AlterColumn<long>(
                name: "ParticipantId",
                table: "Stats",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Participants_ParticipantId",
                table: "Stats",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id");
        }
    }
}
