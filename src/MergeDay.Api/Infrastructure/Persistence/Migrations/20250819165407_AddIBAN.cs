using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MergeDayApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIBAN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillItems_Bills_BillId",
                table: "BillItems");

            migrationBuilder.DropIndex(
                name: "IX_BillItems_BillId",
                table: "BillItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Bills",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BillItems",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "BillId1",
                table: "BillItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "IBANs",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateIndex(
                name: "IX_BillItems_BillId1",
                table: "BillItems",
                column: "BillId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BillItems_Bills_BillId1",
                table: "BillItems",
                column: "BillId1",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillItems_Bills_BillId1",
                table: "BillItems");

            migrationBuilder.DropIndex(
                name: "IX_BillItems_BillId1",
                table: "BillItems");

            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "BillId1",
                table: "BillItems");

            migrationBuilder.DropColumn(
                name: "IBANs",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Bills",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "BillItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_BillItems_BillId",
                table: "BillItems",
                column: "BillId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillItems_Bills_BillId",
                table: "BillItems",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
