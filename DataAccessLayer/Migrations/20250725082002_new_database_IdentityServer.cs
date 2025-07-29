using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class new_database_IdentityServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Users_CreatedById",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_CreatedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_DeletedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UpdatedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Users_CreatedById",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Users_DeletedById",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Users_UpdatedById",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_CreatedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_DeletedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_UpdatedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_Users_CreatedById",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_Users_DeletedById",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_Users_UpdatedById",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_Users_CreatedById",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_Users_DeletedById",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_Users_UpdatedById",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_CreatedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_DeletedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_UpdatedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_Users_CreatedById",
                table: "PersonalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_Users_DeletedById",
                table: "PersonalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_Users_UpdatedById",
                table: "PersonalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_Users_CreatedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_Users_DeletedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_Users_UpdatedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Users_CreatedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Users_DeletedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Users_UpdatedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_DeletedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_UpdatedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_CreatedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_DeletedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_UpdatedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_UserId",
                table: "UserBusinessMappings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_CreatedById",
                table: "ActivityLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_CreatedById",
                table: "Addresses",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_DeletedById",
                table: "Addresses",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_UpdatedById",
                table: "Addresses",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_CreatedById",
                table: "AspNetRoles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_DeletedById",
                table: "AspNetRoles",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_UpdatedById",
                table: "AspNetRoles",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_AspNetUsers_CreatedById",
                table: "Businesses",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_AspNetUsers_DeletedById",
                table: "Businesses",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_AspNetUsers_UpdatedById",
                table: "Businesses",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_AspNetUsers_CreatedById",
                table: "LedgerTransactions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_AspNetUsers_DeletedById",
                table: "LedgerTransactions",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_AspNetUsers_UpdatedById",
                table: "LedgerTransactions",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_AspNetUsers_CreatedById",
                table: "Parties",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_AspNetUsers_DeletedById",
                table: "Parties",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_AspNetUsers_UpdatedById",
                table: "Parties",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetUsers_CreatedById",
                table: "Permissions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetUsers_DeletedById",
                table: "Permissions",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetUsers_UpdatedById",
                table: "Permissions",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_AspNetUsers_CreatedById",
                table: "PersonalDetails",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_AspNetUsers_DeletedById",
                table: "PersonalDetails",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_AspNetUsers_UpdatedById",
                table: "PersonalDetails",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_AspNetUsers_CreatedById",
                table: "ReferenceDataEntities",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_AspNetUsers_DeletedById",
                table: "ReferenceDataEntities",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_AspNetUsers_UpdatedById",
                table: "ReferenceDataEntities",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_AspNetUsers_CreatedById",
                table: "RolePermissionMappings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_AspNetUsers_DeletedById",
                table: "RolePermissionMappings",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_AspNetUsers_UpdatedById",
                table: "RolePermissionMappings",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_AspNetUsers_CreatedById",
                table: "Roles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_AspNetUsers_DeletedById",
                table: "Roles",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_AspNetUsers_UpdatedById",
                table: "Roles",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_CreatedById",
                table: "UserBusinessMappings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_DeletedById",
                table: "UserBusinessMappings",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_UpdatedById",
                table: "UserBusinessMappings",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_UserId",
                table: "UserBusinessMappings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_CreatedById",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_CreatedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_DeletedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_UpdatedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_CreatedById",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_DeletedById",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_UpdatedById",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_AspNetUsers_CreatedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_AspNetUsers_DeletedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_AspNetUsers_UpdatedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_AspNetUsers_CreatedById",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_AspNetUsers_DeletedById",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_AspNetUsers_UpdatedById",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_AspNetUsers_CreatedById",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_AspNetUsers_DeletedById",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_AspNetUsers_UpdatedById",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetUsers_CreatedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetUsers_DeletedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetUsers_UpdatedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_AspNetUsers_CreatedById",
                table: "PersonalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_AspNetUsers_DeletedById",
                table: "PersonalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_AspNetUsers_UpdatedById",
                table: "PersonalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_AspNetUsers_CreatedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_AspNetUsers_DeletedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_AspNetUsers_UpdatedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_AspNetUsers_CreatedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_AspNetUsers_DeletedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_AspNetUsers_UpdatedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_AspNetUsers_CreatedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_AspNetUsers_DeletedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_AspNetUsers_UpdatedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_CreatedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_DeletedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_UpdatedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_AspNetUsers_UserId",
                table: "UserBusinessMappings");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: true),
                    ProfileAttachmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsUserRegistered = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNumber = table.Column<long>(type: "bigint", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Attachments_ProfileAttachmentId",
                        column: x => x.ProfileAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApplicationUserId",
                table: "Users",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfileAttachmentId",
                table: "Users",
                column: "ProfileAttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Users_CreatedById",
                table: "ActivityLogs",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_CreatedById",
                table: "Addresses",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_DeletedById",
                table: "Addresses",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UpdatedById",
                table: "Addresses",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Users_CreatedById",
                table: "AspNetRoles",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Users_DeletedById",
                table: "AspNetRoles",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Users_UpdatedById",
                table: "AspNetRoles",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_CreatedById",
                table: "Businesses",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_DeletedById",
                table: "Businesses",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_UpdatedById",
                table: "Businesses",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_Users_CreatedById",
                table: "LedgerTransactions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_Users_DeletedById",
                table: "LedgerTransactions",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_Users_UpdatedById",
                table: "LedgerTransactions",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_Users_CreatedById",
                table: "Parties",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_Users_DeletedById",
                table: "Parties",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_Users_UpdatedById",
                table: "Parties",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_CreatedById",
                table: "Permissions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_DeletedById",
                table: "Permissions",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_UpdatedById",
                table: "Permissions",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_Users_CreatedById",
                table: "PersonalDetails",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_Users_DeletedById",
                table: "PersonalDetails",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_Users_UpdatedById",
                table: "PersonalDetails",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_Users_CreatedById",
                table: "ReferenceDataEntities",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_Users_DeletedById",
                table: "ReferenceDataEntities",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_Users_UpdatedById",
                table: "ReferenceDataEntities",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Users_CreatedById",
                table: "RolePermissionMappings",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Users_DeletedById",
                table: "RolePermissionMappings",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Users_UpdatedById",
                table: "RolePermissionMappings",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_DeletedById",
                table: "Roles",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_UpdatedById",
                table: "Roles",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_CreatedById",
                table: "UserBusinessMappings",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_DeletedById",
                table: "UserBusinessMappings",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_UpdatedById",
                table: "UserBusinessMappings",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_UserId",
                table: "UserBusinessMappings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
