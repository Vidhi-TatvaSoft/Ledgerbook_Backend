using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class party_LedgerTransaction_table_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PartyTypId = table.Column<int>(type: "int", nullable: false),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GSTIN = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PartyTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parties_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parties_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Parties_ReferenceDataValues_PartyTypeId",
                        column: x => x.PartyTypeId,
                        principalTable: "ReferenceDataValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parties_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Parties_Users_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parties_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LedgerTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_ReferenceDataValues_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "ReferenceDataValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_Users_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LedgerTransactions_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_CreatedById",
                table: "LedgerTransactions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_DeletedById",
                table: "LedgerTransactions",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_PartyId",
                table: "LedgerTransactions",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_PaymentMethodId",
                table: "LedgerTransactions",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_UpdatedById",
                table: "LedgerTransactions",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_AddressId",
                table: "Parties",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_BusinessId",
                table: "Parties",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_CreatedById",
                table: "Parties",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_DeletedById",
                table: "Parties",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_PartyTypeId",
                table: "Parties",
                column: "PartyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_UpdatedById",
                table: "Parties",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LedgerTransactions");

            migrationBuilder.DropTable(
                name: "Parties");
        }
    }
}
