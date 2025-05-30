using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentSwipe.Migrations
{
    /// <inheritdoc />
    public partial class updateFrontEnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Expectations",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HousingDescription",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyRent",
                table: "Profiles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RentSplitPlan",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoommateType",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expectations",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "HousingDescription",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "MonthlyRent",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RentSplitPlan",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "RoommateType",
                table: "Profiles");
        }
    }
}
