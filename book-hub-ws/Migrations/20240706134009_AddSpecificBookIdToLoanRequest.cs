using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_hub_ws.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecificBookIdToLoanRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecificBookId",
                table: "LoanRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_SpecificBookId",
                table: "LoanRequests",
                column: "SpecificBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRequests_Books_SpecificBookId",
                table: "LoanRequests",
                column: "SpecificBookId",
                principalTable: "Books",
                principalColumn: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanRequests_Books_SpecificBookId",
                table: "LoanRequests");

            migrationBuilder.DropIndex(
                name: "IX_LoanRequests_SpecificBookId",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "SpecificBookId",
                table: "LoanRequests");
        }
    }
}
