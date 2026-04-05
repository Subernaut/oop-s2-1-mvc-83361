using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class RedoAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "BranchId1",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentProfileId1",
                table: "AssignmentResults",
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
                name: "IX_Courses_BranchId1",
                table: "Courses",
                column: "BranchId1");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentResults_StudentProfileId1",
                table: "AssignmentResults",
                column: "StudentProfileId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentResults_StudentProfiles_StudentProfileId1",
                table: "AssignmentResults",
                column: "StudentProfileId1",
                principalTable: "StudentProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Branches_BranchId1",
                table: "Courses",
                column: "BranchId1",
                principalTable: "Branches",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentResults_StudentProfiles_StudentProfileId1",
                table: "AssignmentResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Branches_BranchId1",
                table: "Courses");

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
                name: "IX_Courses_BranchId1",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentResults_StudentProfileId1",
                table: "AssignmentResults");

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
                name: "BranchId1",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StudentProfileId1",
                table: "AssignmentResults");
        }
    }
}
