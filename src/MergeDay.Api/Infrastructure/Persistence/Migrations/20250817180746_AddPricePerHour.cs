using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MergeDayApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePerHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHours",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerHours",
                table: "AspNetUsers");
        }
    }
}
