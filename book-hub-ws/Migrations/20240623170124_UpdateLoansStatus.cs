using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_hub_ws.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoansStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "BorrowerUserId",
                table: "Loans",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Loans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BorrowerUserId",
                table: "Loans",
                column: "BorrowerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Users_BorrowerUserId",
                table: "Loans",
                column: "BorrowerUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Users_BorrowerUserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BorrowerUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "BorrowerUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Loans");

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
