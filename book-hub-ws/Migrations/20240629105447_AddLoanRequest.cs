using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace book_hub_ws.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "LoanRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    RequesterUserId = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanRequests_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanRequests_Users_RequesterUserId",
                        column: x => x.RequesterUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_BookId",
                table: "LoanRequests",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_RequesterUserId",
                table: "LoanRequests",
                column: "RequesterUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanRequests");

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
    }
}
