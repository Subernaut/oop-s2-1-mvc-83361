using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedListEnrols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacultyProfiles_FacultyProfiles_FacultyId",
                table: "FacultyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_StudentProfiles_StudentId",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_StudentId",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_FacultyProfiles_FacultyId",
                table: "FacultyProfiles");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "FacultyProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "StudentProfiles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "FacultyProfiles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentProfileId1",
                table: "CourseEnrolments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_IdentityUserId",
                table: "StudentProfiles",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyProfiles_IdentityUserId",
                table: "FacultyProfiles",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrolments_StudentProfileId1",
                table: "CourseEnrolments",
                column: "StudentProfileId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_StudentProfiles_StudentProfileId1",
                table: "CourseEnrolments",
                column: "StudentProfileId1",
                principalTable: "StudentProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacultyProfiles_AspNetUsers_IdentityUserId",
                table: "FacultyProfiles",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_AspNetUsers_IdentityUserId",
                table: "StudentProfiles",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_StudentProfiles_StudentProfileId1",
                table: "CourseEnrolments");

            migrationBuilder.DropForeignKey(
                name: "FK_FacultyProfiles_AspNetUsers_IdentityUserId",
                table: "FacultyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_AspNetUsers_IdentityUserId",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_IdentityUserId",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_FacultyProfiles_IdentityUserId",
                table: "FacultyProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrolments_StudentProfileId1",
                table: "CourseEnrolments");

            migrationBuilder.DropColumn(
                name: "StudentProfileId1",
                table: "CourseEnrolments");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "StudentProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "FacultyProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "FacultyProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_StudentId",
                table: "StudentProfiles",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyProfiles_FacultyId",
                table: "FacultyProfiles",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacultyProfiles_FacultyProfiles_FacultyId",
                table: "FacultyProfiles",
                column: "FacultyId",
                principalTable: "FacultyProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_StudentProfiles_StudentId",
                table: "StudentProfiles",
                column: "StudentId",
                principalTable: "StudentProfiles",
                principalColumn: "Id");
        }
    }
}
