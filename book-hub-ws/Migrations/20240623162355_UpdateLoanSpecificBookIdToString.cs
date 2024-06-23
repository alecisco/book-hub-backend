using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_hub_ws.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoanSpecificBookIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Books_SpecificBookId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_SpecificBookId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "SpecificBookId",
                table: "Loans");

            migrationBuilder.AddColumn<string>(
                name: "SpecificBookTitle",
                table: "Loans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificBookTitle",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "SpecificBookId",
                table: "Loans",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_SpecificBookId",
                table: "Loans",
                column: "SpecificBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Books_SpecificBookId",
                table: "Loans",
                column: "SpecificBookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
