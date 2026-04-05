using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Wiped5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignmentId1",
                table: "AssignmentResults",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentResults_AssignmentId1",
                table: "AssignmentResults",
                column: "AssignmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentResults_Assignments_AssignmentId1",
                table: "AssignmentResults",
                column: "AssignmentId1",
                principalTable: "Assignments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentResults_Assignments_AssignmentId1",
                table: "AssignmentResults");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentResults_AssignmentId1",
                table: "AssignmentResults");

            migrationBuilder.DropColumn(
                name: "AssignmentId1",
                table: "AssignmentResults");
        }
    }
}
