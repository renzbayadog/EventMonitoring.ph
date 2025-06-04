using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventMonitoring.ph.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnumerationGroup",
                columns: table => new
                {
                    EnumerationGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnumerationGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnumerationGroupDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumerationGroup", x => x.EnumerationGroupId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(50)", nullable: false),
                    MiddleInitial = table.Column<string>(type: "varchar(5)", nullable: true),
                    LastName = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "date", nullable: true),
                    UserName = table.Column<string>(type: "varchar(50)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailAddress = table.Column<string>(type: "varchar(225)", maxLength: 256, nullable: false),
                    NormalizedEmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(max)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "varchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_User_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "EnumerationGroupValue",
                columns: table => new
                {
                    EnumerationGroupValueName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnumerationGroupValueId = table.Column<int>(type: "int", nullable: false),
                    EnumerationGroupValueDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnumerationGroupId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumerationGroupValue", x => x.EnumerationGroupValueName);
                    table.ForeignKey(
                        name: "FK_EnumerationGroupValue_EnumerationGroup_EnumerationGroupId",
                        column: x => x.EnumerationGroupId,
                        principalTable: "EnumerationGroup",
                        principalColumn: "EnumerationGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityTracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTracker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTracker_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ActivityTracker_User_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ActivityTracker_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ActivityTracker_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuName = table.Column<string>(type: "varchar(50)", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    MenuDescription = table.Column<string>(type: "varchar(225)", nullable: true),
                    MenuSequence = table.Column<int>(type: "int", nullable: true),
                    ProcessControl = table.Column<bool>(type: "bit", nullable: false),
                    Link = table.Column<string>(type: "varchar(225)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_Menu_Menu_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menu",
                        principalColumn: "MenuId");
                    table.ForeignKey(
                        name: "FK_Menu_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Menu_User_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Menu_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleDescription = table.Column<string>(type: "varchar(225)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "date", nullable: true),
                    RoleName = table.Column<string>(type: "varchar(50)", maxLength: 256, nullable: false),
                    NormalizedRoleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(max)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Role_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Role_User_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Role_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    UserClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "varchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "varchar(max)", nullable: true),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.UserClaimId);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "varchar(50)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.ProviderKey, x.LoginProvider });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TokenName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TokenValue = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.TokenName });
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                columns: table => new
                {
                    RoleClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "varchar(50)", nullable: true),
                    ClaimValue = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => x.RoleClaimId);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenu",
                columns: table => new
                {
                    RoleMenuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: true),
                    DeletedById = table.Column<int>(type: "int", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenu", x => x.RoleMenuId);
                    table.ForeignKey(
                        name: "FK_RoleMenu_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menu",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenu_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenu_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_RoleMenu_User_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_RoleMenu_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTracker_CreatedById",
                table: "ActivityTracker",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTracker_DeletedById",
                table: "ActivityTracker",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTracker_UpdatedById",
                table: "ActivityTracker",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTracker_UserId",
                table: "ActivityTracker",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EnumerationGroupValue_EnumerationGroupId",
                table: "EnumerationGroupValue",
                column: "EnumerationGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_CreatedById",
                table: "Menu",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_DeletedById",
                table: "Menu",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_ParentId",
                table: "Menu",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_UpdatedById",
                table: "Menu",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatedById",
                table: "Role",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Role_DeletedById",
                table: "Role",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UpdatedById",
                table: "Role",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedRoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_RoleId",
                table: "RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_CreatedById",
                table: "RoleMenu",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_DeletedById",
                table: "RoleMenu",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_MenuId",
                table: "RoleMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_RoleId",
                table: "RoleMenu",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_UpdatedById",
                table: "RoleMenu",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedById",
                table: "User",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_DeletedById",
                table: "User",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_UpdatedById",
                table: "User",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId1",
                table: "UserClaim",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId1",
                table: "UserLogin",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId1",
                table: "UserRole",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityTracker");

            migrationBuilder.DropTable(
                name: "EnumerationGroupValue");

            migrationBuilder.DropTable(
                name: "RoleClaim");

            migrationBuilder.DropTable(
                name: "RoleMenu");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "EnumerationGroup");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
