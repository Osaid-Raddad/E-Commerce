using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changeIdentityuserrolename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersRoles",
                table: "UsersRoles");

            migrationBuilder.RenameTable(
                name: "UsersRoles",
                newName: "UserRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UsersRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersRoles",
                table: "UsersRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
