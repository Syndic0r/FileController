using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileController.Migrations
{
    /// <inheritdoc />
    public partial class Added_Transaction_SortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_IBAN",
                table: "BankAccounts");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "StatementTransaction",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "StatementTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_IBAN",
                table: "BankAccounts",
                column: "IBAN",
                unique: true);
        }
    }
}
