using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Wiped3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AssignmentResults_AssignmentResultId1",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ExamResults_ExamResultId1",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_StudentProfiles_StudentProfileId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_ExamResultId1",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_StudentProfileId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_AssignmentResultId1",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ExamResultId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamResultId1",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StudentProfileId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "AssignmentResultId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "AssignmentResultId1",
                table: "Assignments");

            migrationBuilder.AddColumn<int>(
                name: "StudentProfileId1",
                table: "ExamResults",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_StudentProfileId1",
                table: "ExamResults",
                column: "StudentProfileId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_StudentProfiles_StudentProfileId1",
                table: "ExamResults",
                column: "StudentProfileId1",
                principalTable: "StudentProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_StudentProfiles_StudentProfileId1",
                table: "ExamResults");

            migrationBuilder.DropIndex(
                name: "IX_ExamResults_StudentProfileId1",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "StudentProfileId1",
                table: "ExamResults");

            migrationBuilder.AddColumn<int>(
                name: "ExamResultId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamResultId1",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentProfileId",
                table: "Exams",
                type: "int",
                nullable: true);

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
                name: "IX_Exams_ExamResultId1",
                table: "Exams",
                column: "ExamResultId1");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_StudentProfileId",
                table: "Exams",
                column: "StudentProfileId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ExamResults_ExamResultId1",
                table: "Exams",
                column: "ExamResultId1",
                principalTable: "ExamResults",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_StudentProfiles_StudentProfileId",
                table: "Exams",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id");
        }
    }
}
