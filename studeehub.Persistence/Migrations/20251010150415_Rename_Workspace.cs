using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class Rename_Workspace : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsActive",
				table: "Streaks",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8378), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8378) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8388), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8388) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8390), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8390) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8392), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8393) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8395), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8395) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEN4ErbMofMbnPjlth+triRNqYz9zMTA3hSxYMv4dcd4BwbrKNsKctp0qaCjXp/+U0g==", new DateTime(2025, 10, 17, 15, 4, 14, 133, DateTimeKind.Utc).AddTicks(2944) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEGsNs0qj766aZvHrNlDt9xEuwD0g/wdecrHCLdIDzsRwmnx8v7XXSMQqTUSvDUdiEg==", new DateTime(2025, 10, 17, 15, 4, 14, 85, DateTimeKind.Utc).AddTicks(3648) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8310), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8310) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8328), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8328) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8335), new DateTime(2025, 10, 10, 15, 4, 14, 182, DateTimeKind.Utc).AddTicks(8335) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsActive",
				table: "Streaks");

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2025), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2026) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2034), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2034) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2036), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2036) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2038), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2038) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2040), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2040) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAECzzTN77SSVqYTwlnNB8dy9FWCcnurLh/SSjxAeWJDhBtl2L1nlQVTnyh/00oQU8+A==", new DateTime(2025, 10, 16, 9, 10, 16, 687, DateTimeKind.Utc).AddTicks(3371) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEBKhYRyMPvAbNvk0v3GZiE9cxJenWzaU6QnVEk3FKppxJiXyteukdvADjyBW6oyC0g==", new DateTime(2025, 10, 16, 9, 10, 16, 639, DateTimeKind.Utc).AddTicks(2200) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1969), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1973) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1984), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1984) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1990), new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1990) });
		}
	}
}
