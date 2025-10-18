using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class Rename_Workspace_again : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9727), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9727) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9736), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9737) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9740), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9740) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9742), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9742) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9744), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9744) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEMIF80J4ouZ79qqdQd4KrHfT63BD86aFi90COXTFH+PWKu4dTCWNj5SUrvVlzEZqoA==", new DateTime(2025, 10, 17, 15, 8, 3, 350, DateTimeKind.Utc).AddTicks(4760) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEI76xDe2aX235Dxfv+LgXrI7q5kclBvoskB7j7y04sRmYpcOnOjnIGP0fbcetO0Vaw==", new DateTime(2025, 10, 17, 15, 8, 3, 302, DateTimeKind.Utc).AddTicks(7909) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9667), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9668) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9684), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9684) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9691), new DateTime(2025, 10, 10, 15, 8, 3, 397, DateTimeKind.Utc).AddTicks(9691) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
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
	}
}
