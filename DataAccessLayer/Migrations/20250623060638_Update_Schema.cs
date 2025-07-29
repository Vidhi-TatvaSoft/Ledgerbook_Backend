using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Update_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_CreatedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_ReferenceDataValues_BusinessTypeId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_CreatedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_CreatedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_Users_CreatedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataValues_ReferenceDataEntities_EntityTypeId",
                table: "ReferenceDataValues");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Permissions_PermissionId",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Roles_RoleId",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Users_CreatedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Businesses_BusinessId",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Roles_RoleId",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_CreatedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_UserId",
                table: "UserBusinessMappings");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EntityValue",
                table: "ReferenceDataValues",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                table: "ReferenceDataEntities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Permission",
                table: "Permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                table: "Businesses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_CreatedById",
                table: "Addresses",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_ReferenceDataValues_BusinessTypeId",
                table: "Businesses",
                column: "BusinessTypeId",
                principalTable: "ReferenceDataValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_CreatedById",
                table: "Businesses",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_CreatedById",
                table: "Permissions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_Users_CreatedById",
                table: "ReferenceDataEntities",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataValues_ReferenceDataEntities_EntityTypeId",
                table: "ReferenceDataValues",
                column: "EntityTypeId",
                principalTable: "ReferenceDataEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Permissions_PermissionId",
                table: "RolePermissionMappings",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Roles_RoleId",
                table: "RolePermissionMappings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Users_CreatedById",
                table: "RolePermissionMappings",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Businesses_BusinessId",
                table: "UserBusinessMappings",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Roles_RoleId",
                table: "UserBusinessMappings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_CreatedById",
                table: "UserBusinessMappings",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_UserId",
                table: "UserBusinessMappings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_CreatedById",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_ReferenceDataValues_BusinessTypeId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_CreatedById",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_CreatedById",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataEntities_Users_CreatedById",
                table: "ReferenceDataEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceDataValues_ReferenceDataEntities_EntityTypeId",
                table: "ReferenceDataValues");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Permissions_PermissionId",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Roles_RoleId",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Users_CreatedById",
                table: "RolePermissionMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Businesses_BusinessId",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Roles_RoleId",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_CreatedById",
                table: "UserBusinessMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBusinessMappings_Users_UserId",
                table: "UserBusinessMappings");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityValue",
                table: "ReferenceDataValues",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                table: "ReferenceDataEntities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Permission",
                table: "Permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                table: "Businesses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_CreatedById",
                table: "Addresses",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_ReferenceDataValues_BusinessTypeId",
                table: "Businesses",
                column: "BusinessTypeId",
                principalTable: "ReferenceDataValues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_CreatedById",
                table: "Businesses",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_CreatedById",
                table: "Permissions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataEntities_Users_CreatedById",
                table: "ReferenceDataEntities",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceDataValues_ReferenceDataEntities_EntityTypeId",
                table: "ReferenceDataValues",
                column: "EntityTypeId",
                principalTable: "ReferenceDataEntities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Permissions_PermissionId",
                table: "RolePermissionMappings",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Roles_RoleId",
                table: "RolePermissionMappings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Users_CreatedById",
                table: "RolePermissionMappings",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Businesses_BusinessId",
                table: "UserBusinessMappings",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Roles_RoleId",
                table: "UserBusinessMappings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_CreatedById",
                table: "UserBusinessMappings",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBusinessMappings_Users_UserId",
                table: "UserBusinessMappings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
