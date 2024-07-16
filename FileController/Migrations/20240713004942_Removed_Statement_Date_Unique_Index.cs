using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileController.Migrations
{
    /// <inheritdoc />
    public partial class Removed_Statement_Date_Unique_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountStatements_CreationDate",
                table: "AccountStatements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_CreationDate",
                table: "AccountStatements",
                column: "CreationDate",
                unique: true);
        }
    }
}
