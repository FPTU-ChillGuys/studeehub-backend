using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace studeehub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnuseEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Flashcards");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Workspaces");

            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"));

            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"));

            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"));

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3042), new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3042) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3051), new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3051) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAECmM8+Cexrcr1wecnYQR8UeeyUY99EHADYuzAilYT15yzk59sRqFZuVhN3pomLTNPQ==", new DateTime(2025, 10, 31, 15, 15, 24, 22, DateTimeKind.Utc).AddTicks(6757) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEKtb/poWWXjseAxA7ErbghrjWATO4aEwbD5rbriumnYjueaWSUn5RKmgQKsILS7JzQ==", new DateTime(2025, 10, 31, 15, 15, 23, 975, DateTimeKind.Utc).AddTicks(1147) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkSpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Workspaces_WorkSpaceId",
                        column: x => x.WorkSpaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flashcards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkSpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flashcards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flashcards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flashcards_Workspaces_WorkSpaceId",
                        column: x => x.WorkSpaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkSpaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notes_Workspaces_WorkSpaceId",
                        column: x => x.WorkSpaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9156), new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9157) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9171), new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9171) });

            migrationBuilder.InsertData(
                table: "Achievements",
                columns: new[] { "Id", "Code", "ConditionType", "ConditionValue", "CreatedAt", "DeletedAt", "Description", "IsActive", "IsDeleted", "Name", "RewardType", "RewardValue", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"), "FIRST_NOTE", "NoteCreated", 1, new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9174), null, "Create your first note.", true, false, "First Note Created", "Badge", 1, new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9175) },
                    { new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"), "FLASHCARDS_REVIEWED_10", "FlashcardReviewed", 10, new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9182), null, "Review 10 flashcards.", true, false, "10 Flashcards Reviewed", "XP", 100, new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9182) },
                    { new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"), "DOCUMENTS_UPLOADED_10", "DocumentUpload", 10, new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9184), null, "Upload 10 documents.", true, false, "10 Documents Uploaded", "XP", 200, new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9184) }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEGO54MWOaFy7WXUvtZG4J7jOoOQxX/SC4gRy008Sj4VRdJL80+MWD8KHg/0BPZuOPg==", new DateTime(2025, 10, 31, 14, 17, 16, 397, DateTimeKind.Utc).AddTicks(1193) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEIGc8VzO6zznOW02qjvEB0c20mFj2QGL8SI12WmMl2iqXKTR4bCleYDQdhqJRYb80w==", new DateTime(2025, 10, 31, 14, 17, 16, 347, DateTimeKind.Utc).AddTicks(5294) });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_WorkSpaceId",
                table: "Documents",
                column: "WorkSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_UserId",
                table: "Flashcards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_WorkSpaceId",
                table: "Flashcards",
                column: "WorkSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_WorkSpaceId",
                table: "Notes",
                column: "WorkSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UserId",
                table: "Workspaces",
                column: "UserId");
        }
    }
}
