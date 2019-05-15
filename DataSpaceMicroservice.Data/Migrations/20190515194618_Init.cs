using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataSpaceMicroservice.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Firstname = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    AvatarImageUrl = table.Column<string>(nullable: true),
                    DataSpaceDirName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DSNodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifiedTime = table.Column<DateTime>(nullable: false),
                    NodeType = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DSNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DSNodes_Accounts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DSDirectories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NodeId = table.Column<int>(nullable: false),
                    ParentDirectoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DSDirectories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DSDirectories_DSNodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "DSNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DSDirectories_DSDirectories_ParentDirectoryId",
                        column: x => x.ParentDirectoryId,
                        principalTable: "DSDirectories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DSFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MimeType = table.Column<string>(nullable: true),
                    NodeId = table.Column<int>(nullable: false),
                    ParentDirectoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DSFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DSFiles_DSNodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "DSNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DSFiles_DSDirectories_ParentDirectoryId",
                        column: x => x.ParentDirectoryId,
                        principalTable: "DSDirectories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileAccountShares",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAccountShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAccountShares_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileAccountShares_DSFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "DSFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PhoneNumber",
                table: "Accounts",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DSDirectories_NodeId",
                table: "DSDirectories",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DSDirectories_ParentDirectoryId",
                table: "DSDirectories",
                column: "ParentDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DSFiles_NodeId",
                table: "DSFiles",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DSFiles_ParentDirectoryId",
                table: "DSFiles",
                column: "ParentDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DSNodes_OwnerId",
                table: "DSNodes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAccountShares_AccountId",
                table: "FileAccountShares",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAccountShares_FileId",
                table: "FileAccountShares",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileAccountShares");

            migrationBuilder.DropTable(
                name: "DSFiles");

            migrationBuilder.DropTable(
                name: "DSDirectories");

            migrationBuilder.DropTable(
                name: "DSNodes");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
