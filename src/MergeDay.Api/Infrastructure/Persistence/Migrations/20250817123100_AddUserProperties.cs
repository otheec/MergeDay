using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MergeDayApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FakturoidClientId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FakturoidClientSecret",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FakturoidSlug",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FakturoidClientId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FakturoidClientSecret",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FakturoidSlug",
                table: "AspNetUsers");
        }
    }
}
