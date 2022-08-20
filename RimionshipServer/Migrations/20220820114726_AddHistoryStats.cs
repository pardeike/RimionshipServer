using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AddHistoryStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    table.PrimaryKey("PK_HistoryStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryStats_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoryStats_UserId",
                table: "HistoryStats",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryStats");
        }
    }
}
