using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class GameDataServiceMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseEntry",
                columns: table => new
                {
                    UId = table.Column<string>(type: "TEXT", nullable: false),
                    TimeTicks = table.Column<long>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntry", x => x.UId);
                });

            migrationBuilder.CreateTable(
                name: "Caravans",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caravans", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Caravans_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Colonists",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colonists", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Colonists_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColonistsKilled",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColonistsKilled", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_ColonistsKilled_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColonistsNeedTending",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColonistsNeedTending", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_ColonistsNeedTending_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conditions",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Conditions_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DamageDealt",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageDealt", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_DamageDealt_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DamageTakenPawns",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageTakenPawns", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_DamageTakenPawns_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DamageTakenThings",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageTakenThings", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_DamageTakenThings_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DownedColonists",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownedColonists", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_DownedColonists_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Electricity",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Electricity", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Electricity_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enemies",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enemies", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Enemies_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fire",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fire", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Fire_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Food_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GreatestPopulation",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GreatestPopulation", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_GreatestPopulation_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InGameHours",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InGameHours", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_InGameHours_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapCount",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapCount", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_MapCount_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalConditions",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalConditions", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_MedicalConditions_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medicine",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Medicine_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentalColonists",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentalColonists", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_MentalColonists_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NumRaidsEnemy",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumRaidsEnemy", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_NumRaidsEnemy_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NumThreatBigs",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumThreatBigs", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_NumThreatBigs_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prisoners",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prisoners", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Prisoners_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Rooms_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TamedAnimals",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TamedAnimals", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_TamedAnimals_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Temperature",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temperature", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Temperature_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Visitors_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wealth",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wealth", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_Wealth_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponDps",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponDps", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_WeaponDps_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WildAnimals",
                columns: table => new
                {
                    HiddenId = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WildAnimals", x => x.HiddenId);
                    table.ForeignKey(
                        name: "FK_WildAnimals_BaseEntry_HiddenId",
                        column: x => x.HiddenId,
                        principalTable: "BaseEntry",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntry_Id",
                table: "BaseEntry",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Caravans");

            migrationBuilder.DropTable(
                name: "Colonists");

            migrationBuilder.DropTable(
                name: "ColonistsKilled");

            migrationBuilder.DropTable(
                name: "ColonistsNeedTending");

            migrationBuilder.DropTable(
                name: "Conditions");

            migrationBuilder.DropTable(
                name: "DamageDealt");

            migrationBuilder.DropTable(
                name: "DamageTakenPawns");

            migrationBuilder.DropTable(
                name: "DamageTakenThings");

            migrationBuilder.DropTable(
                name: "DownedColonists");

            migrationBuilder.DropTable(
                name: "Electricity");

            migrationBuilder.DropTable(
                name: "Enemies");

            migrationBuilder.DropTable(
                name: "Fire");

            migrationBuilder.DropTable(
                name: "Food");

            migrationBuilder.DropTable(
                name: "GreatestPopulation");

            migrationBuilder.DropTable(
                name: "InGameHours");

            migrationBuilder.DropTable(
                name: "MapCount");

            migrationBuilder.DropTable(
                name: "MedicalConditions");

            migrationBuilder.DropTable(
                name: "Medicine");

            migrationBuilder.DropTable(
                name: "MentalColonists");

            migrationBuilder.DropTable(
                name: "NumRaidsEnemy");

            migrationBuilder.DropTable(
                name: "NumThreatBigs");

            migrationBuilder.DropTable(
                name: "Prisoners");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "TamedAnimals");

            migrationBuilder.DropTable(
                name: "Temperature");

            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.DropTable(
                name: "Wealth");

            migrationBuilder.DropTable(
                name: "WeaponDps");

            migrationBuilder.DropTable(
                name: "WildAnimals");

            migrationBuilder.DropTable(
                name: "BaseEntry");
        }
    }
}
