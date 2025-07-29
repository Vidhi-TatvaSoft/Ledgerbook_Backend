using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class personalDetails_table_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUserRegistered",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PersonDetailId",
                table: "UserBusinessMappings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonalDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    MobileNumber = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalDetails_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PersonalDetails_Users_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonalDetails_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBusinessMappings_PersonDetailId",
                table: "UserBusinessMappings",
                column: "PersonDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_CreatedById",
                table: "PersonalDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_DeletedById",
                table: "PersonalDetails",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_UpdatedById",
                table: "PersonalDetails",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_PersonalDetails_PersonDetailId",
                table: "UserBusinessMappings",
                column: "PersonDetailId",
                principalTable: "PersonalDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_PersonalDetails_PersonDetailId",
                table: "UserBusinessMappings");

            migrationBuilder.DropTable(
                name: "PersonalDetails");

            migrationBuilder.DropIndex(
                name: "IX_UserBusinessMappings_PersonDetailId",
                table: "UserBusinessMappings");

            migrationBuilder.DropColumn(
                name: "IsUserRegistered",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PersonDetailId",
                table: "UserBusinessMappings");
        }
    }
}
