using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AddGraphRotationData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GraphRotationData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RotationName = table.Column<string>(type: "TEXT", nullable: false),
                    TimeToDisplay = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphRotationData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraphDataGraphRotationData",
                columns: table => new
                {
                    InRotationsId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToRotateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphDataGraphRotationData", x => new { x.InRotationsId, x.ToRotateId });
                    table.ForeignKey(
                        name: "FK_GraphDataGraphRotationData_GraphData_ToRotateId",
                        column: x => x.ToRotateId,
                        principalTable: "GraphData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GraphDataGraphRotationData_GraphRotationData_InRotationsId",
                        column: x => x.InRotationsId,
                        principalTable: "GraphRotationData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraphDataGraphRotationData_ToRotateId",
                table: "GraphDataGraphRotationData",
                column: "ToRotateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraphDataGraphRotationData");

            migrationBuilder.DropTable(
                name: "GraphRotationData");
        }
    }
}
