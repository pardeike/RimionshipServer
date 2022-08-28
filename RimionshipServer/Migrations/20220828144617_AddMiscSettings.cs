using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AddMiscSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BroadcastMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BroadcastMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Punishment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FinalPauseInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxThoughtFactor = table.Column<float>(type: "REAL", nullable: false),
                    MinThoughtFactor = table.Column<float>(type: "REAL", nullable: false),
                    StartPauseInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    fk_Settings = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punishment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Punishment_Settings_fk_Settings",
                        column: x => x.fk_Settings,
                        principalTable: "Settings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rising",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxFreeColonistCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RisingCooldown = table.Column<int>(type: "INTEGER", nullable: false),
                    RisingInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    RisingIntervalMinimum = table.Column<int>(type: "INTEGER", nullable: false),
                    RisingReductionPerColonist = table.Column<int>(type: "INTEGER", nullable: false),
                    fk_Settings = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rising", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rising_Settings_fk_Settings",
                        column: x => x.fk_Settings,
                        principalTable: "Settings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Traits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BadTraitSuppression = table.Column<float>(type: "REAL", nullable: false),
                    GoodTraitSuppression = table.Column<float>(type: "REAL", nullable: false),
                    ScaleFactor = table.Column<float>(type: "REAL", nullable: false),
                    MaxMeleeSkill = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxMeleeFlames = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxShootingFlames = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxShootingSkill = table.Column<int>(type: "INTEGER", nullable: false),
                    fk_Settings = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Traits_Settings_fk_Settings",
                        column: x => x.fk_Settings,
                        principalTable: "Settings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Punishment_fk_Settings",
                table: "Punishment",
                column: "fk_Settings",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rising_fk_Settings",
                table: "Rising",
                column: "fk_Settings",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Name",
                table: "Settings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Traits_fk_Settings",
                table: "Traits",
                column: "fk_Settings",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BroadcastMessage");

            migrationBuilder.DropTable(
                name: "Punishment");

            migrationBuilder.DropTable(
                name: "Rising");

            migrationBuilder.DropTable(
                name: "Traits");

            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
