using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class TimeStamp : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "IsExpiryNotified",
				table: "Subscriptions",
				newName: "IsPreExpiryNotified");

			migrationBuilder.AddColumn<bool>(
				name: "IsPostExpiryNotified",
				table: "Subscriptions",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<DateTime>(
				name: "PostExpiryNotifiedAt",
				table: "Subscriptions",
				type: "datetime2",
				nullable: true);

			migrationBuilder.AddColumn<DateTime>(
				name: "PreExpiryNotifiedAt",
				table: "Subscriptions",
				type: "datetime2",
				nullable: true);

			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedAt",
				table: "SubscriptionPlans",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "DeletedAt",
				table: "SubscriptionPlans",
				type: "datetime2",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "SubscriptionPlans",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<DateTime>(
				name: "UpdatedAt",
				table: "SubscriptionPlans",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedAt",
				table: "Streaks",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedAt",
				table: "Schedules",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "UpdatedAt",
				table: "Schedules",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "UpdatedAt",
				table: "Documents",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedAt",
				table: "Achievements",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "DeletedAt",
				table: "Achievements",
				type: "datetime2",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Achievements",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<DateTime>(
				name: "UpdatedAt",
				table: "Achievements",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2025), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2026) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2034), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2034) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2036), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2036) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2038), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2038) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2040), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(2040) });

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
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1969), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1973) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1984), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1984) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
				columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1990), null, false, new DateTime(2025, 10, 9, 9, 10, 16, 735, DateTimeKind.Utc).AddTicks(1990) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsPostExpiryNotified",
				table: "Subscriptions");

			migrationBuilder.DropColumn(
				name: "PostExpiryNotifiedAt",
				table: "Subscriptions");

			migrationBuilder.DropColumn(
				name: "PreExpiryNotifiedAt",
				table: "Subscriptions");

			migrationBuilder.DropColumn(
				name: "CreatedAt",
				table: "SubscriptionPlans");

			migrationBuilder.DropColumn(
				name: "DeletedAt",
				table: "SubscriptionPlans");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "SubscriptionPlans");

			migrationBuilder.DropColumn(
				name: "UpdatedAt",
				table: "SubscriptionPlans");

			migrationBuilder.DropColumn(
				name: "CreatedAt",
				table: "Streaks");

			migrationBuilder.DropColumn(
				name: "CreatedAt",
				table: "Schedules");

			migrationBuilder.DropColumn(
				name: "UpdatedAt",
				table: "Schedules");

			migrationBuilder.DropColumn(
				name: "UpdatedAt",
				table: "Documents");

			migrationBuilder.DropColumn(
				name: "CreatedAt",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "DeletedAt",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "UpdatedAt",
				table: "Achievements");

			migrationBuilder.RenameColumn(
				name: "IsPreExpiryNotified",
				table: "Subscriptions",
				newName: "IsExpiryNotified");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEOnSzeWD91AQLu3pKxB4yHId4woT1YkN4o7S6PbbjADw82oWADTbPndnQVJ5XeZhnQ==", new DateTime(2025, 10, 15, 17, 6, 4, 773, DateTimeKind.Utc).AddTicks(9681) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEO9gcdf2lvXRvp2ophM98Zng7dU5TRMp0N+XVKLsoWc7XCR5vLABHUsnjcqAm6KczQ==", new DateTime(2025, 10, 15, 17, 6, 4, 724, DateTimeKind.Utc).AddTicks(6463) });
		}
	}
}
