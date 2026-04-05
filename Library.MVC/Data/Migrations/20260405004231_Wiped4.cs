using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Wiped4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId",
                table: "CourseEnrolments");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId",
                table: "CourseEnrolments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId",
                table: "CourseEnrolments");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_Courses_CourseId",
                table: "CourseEnrolments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
