using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AssignementsRedo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignmentResultId",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignmentResultId1",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssignmentResultId1",
                table: "Assignments",
                column: "AssignmentResultId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AssignmentResults_AssignmentResultId1",
                table: "Assignments",
                column: "AssignmentResultId1",
                principalTable: "AssignmentResults",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AssignmentResults_AssignmentResultId1",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_AssignmentResultId1",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "AssignmentResultId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "AssignmentResultId1",
                table: "Assignments");
        }
    }
}
