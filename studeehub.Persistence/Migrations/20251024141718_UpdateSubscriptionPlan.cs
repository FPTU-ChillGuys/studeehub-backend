using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptionPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxDocuments",
                table: "SubscriptionPlans",
                newName: "FlashcardCreationLimitPerDay");

            migrationBuilder.AddColumn<int>(
                name: "AIQueriesPerDay",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "DiscountPercentage",
                table: "SubscriptionPlans",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "DocumentUploadLimitPerDay",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9174), new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9175) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9182), new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9182) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9184), new DateTime(2025, 10, 24, 14, 17, 16, 447, DateTimeKind.Utc).AddTicks(9184) });

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

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
                columns: new[] { "AIQueriesPerDay", "CreatedAt", "DiscountPercentage", "DocumentUploadLimitPerDay", "FlashcardCreationLimitPerDay", "UpdatedAt" },
                values: new object[] { 0, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), 0f, 50, 0, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
                columns: new[] { "AIQueriesPerDay", "CreatedAt", "DiscountPercentage", "DocumentUploadLimitPerDay", "FlashcardCreationLimitPerDay", "UpdatedAt" },
                values: new object[] { 1000, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), 0f, 1000, 200, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
                columns: new[] { "AIQueriesPerDay", "CreatedAt", "DiscountPercentage", "DocumentUploadLimitPerDay", "FlashcardCreationLimitPerDay", "UpdatedAt" },
                values: new object[] { 1000, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), 0f, 1000, 200, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIQueriesPerDay",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "DocumentUploadLimitPerDay",
                table: "SubscriptionPlans");

            migrationBuilder.RenameColumn(
                name: "FlashcardCreationLimitPerDay",
                table: "SubscriptionPlans",
                newName: "MaxDocuments");

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5001), new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5002) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5010), new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5010) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5012), new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5013) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5016), new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5016) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5018), new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(5018) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEBFnvPlWrUs57+l/H9DDYcwHsfHTZ+QMUk01Ji7fo6Yy9oqXVSYEESRxUZOaL5vrJw==", new DateTime(2025, 10, 19, 16, 16, 4, 497, DateTimeKind.Utc).AddTicks(3739) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAENEnOtD/ZISict5GsZZjDR9btxc0h07v3eChyqOyG8RJunDNYgA6FDfnKHqAzED6JA==", new DateTime(2025, 10, 19, 16, 16, 4, 448, DateTimeKind.Utc).AddTicks(4914) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
                columns: new[] { "CreatedAt", "MaxDocuments", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(4854), 50, new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(4858) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
                columns: new[] { "CreatedAt", "MaxDocuments", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(4870), 1000, new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(4871) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
                columns: new[] { "CreatedAt", "MaxDocuments", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(4952), 1000, new DateTime(2025, 10, 12, 16, 16, 4, 546, DateTimeKind.Utc).AddTicks(4952) });
        }
    }
}
