using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentSwipe.Migrations
{
    /// <inheritdoc />
    public partial class HsUniCourseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Profiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HSCourses",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UniCourses",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UniversityYear",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "HSCourses",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "UniCourses",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "UniversityYear",
                table: "Profiles");
        }
    }
}
