using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MergeDayApi.Migrations
{
    /// <inheritdoc />
    public partial class Notes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuditorNote",
                table: "Absences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestorNote",
                table: "Absences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditorNote",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "RequestorNote",
                table: "Absences");
        }
    }
}
