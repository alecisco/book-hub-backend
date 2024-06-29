using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_hub_ws.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanRequestEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "LoanRequests",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "LoanRequests");
        }
    }
}
