using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileController.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    IBAN = table.Column<string>(type: "TEXT", nullable: false),
                    Bankname = table.Column<string>(type: "TEXT", nullable: false),
                    BLZ = table.Column<int>(type: "INTEGER", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    BIC = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.IBAN);
                });

            migrationBuilder.CreateTable(
                name: "AccountStatements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    Number = table.Column<string>(type: "TEXT", nullable: false),
                    ValueStart = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValueEnd = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    PagesText = table.Column<string>(type: "TEXT", nullable: false),
                    AccountIBAN = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountStatements_BankAccounts_AccountIBAN",
                        column: x => x.AccountIBAN,
                        principalTable: "BankAccounts",
                        principalColumn: "IBAN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatementPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    AccountStatementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatementPage_AccountStatements_AccountStatementId",
                        column: x => x.AccountStatementId,
                        principalTable: "AccountStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatementTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    AccountStatementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatementTransaction_AccountStatements_AccountStatementId",
                        column: x => x.AccountStatementId,
                        principalTable: "AccountStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_AccountIBAN",
                table: "AccountStatements",
                column: "AccountIBAN");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_CreationDate",
                table: "AccountStatements",
                column: "CreationDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatements_Number",
                table: "AccountStatements",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_IBAN",
                table: "BankAccounts",
                column: "IBAN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_Number",
                table: "BankAccounts",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatementPage_AccountStatementId",
                table: "StatementPage",
                column: "AccountStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementTransaction_AccountStatementId",
                table: "StatementTransaction",
                column: "AccountStatementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatementPage");

            migrationBuilder.DropTable(
                name: "StatementTransaction");

            migrationBuilder.DropTable(
                name: "AccountStatements");

            migrationBuilder.DropTable(
                name: "BankAccounts");
        }
    }
}
