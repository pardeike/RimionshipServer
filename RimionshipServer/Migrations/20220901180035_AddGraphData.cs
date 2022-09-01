using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RimionshipServer.Migrations
{
    public partial class AddGraphData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GraphData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Accesscode = table.Column<string>(type: "TEXT", nullable: false),
                    Statt = table.Column<string>(type: "TEXT", nullable: false),
                    Start = table.Column<long>(type: "INTEGER", nullable: false),
                    End = table.Column<long>(type: "INTEGER", nullable: false),
                    IntervalSeconds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraphDataRimionUser",
                columns: table => new
                {
                    InGraphsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersReferenceId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphDataRimionUser", x => new { x.InGraphsId, x.UsersReferenceId });
                    table.ForeignKey(
                        name: "FK_GraphDataRimionUser_AspNetUsers_UsersReferenceId",
                        column: x => x.UsersReferenceId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GraphDataRimionUser_GraphData_InGraphsId",
                        column: x => x.InGraphsId,
                        principalTable: "GraphData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraphDataRimionUser_UsersReferenceId",
                table: "GraphDataRimionUser",
                column: "UsersReferenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraphDataRimionUser");

            migrationBuilder.DropTable(
                name: "GraphData");
        }
    }
}
