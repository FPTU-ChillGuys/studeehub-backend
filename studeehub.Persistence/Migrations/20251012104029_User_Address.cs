using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class User_Address : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Address",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1079), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1080) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1091), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1091) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1094), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1094) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1097), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1098) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1099), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1100) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "Address", "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "", "AQAAAAIAAYagAAAAEK+0UHkir8hlN3Ohf4noeerzmeShs4ot1esI7GdYGCvzN2VuNRl7bSCGsOiL6sUtXA==", new DateTime(2025, 10, 19, 10, 40, 27, 871, DateTimeKind.Utc).AddTicks(9244) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "Address", "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "", "AQAAAAIAAYagAAAAEH8KT8J+aOiN9ic5bPI9cd3GeShXaSLeA/j7FZgv8hox3MQyK4hrvFuLE7bPH78Emw==", new DateTime(2025, 10, 19, 10, 40, 27, 823, DateTimeKind.Utc).AddTicks(2936) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1016), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1017) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1029), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1029) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1036), new DateTime(2025, 10, 12, 10, 40, 27, 920, DateTimeKind.Utc).AddTicks(1036) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Address",
				table: "AspNetUsers");

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(209), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(210) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(221), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(221) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(223), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(224) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(227), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(227) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(230), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(230) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAELd05YHvMYJWiNDrqWfWVHP9DLsHV8BXftZet6JNeAwpO8xcRt8UgK6B7j1LvMTbvQ==", new DateTime(2025, 10, 19, 5, 21, 42, 577, DateTimeKind.Utc).AddTicks(6923) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEJYZhtqNAPIml/jiAF6gxjAqxof/vEfHAN29MRb/+yOO/rCWsWC25WKiUpA7JYd2MQ==", new DateTime(2025, 10, 19, 5, 21, 42, 528, DateTimeKind.Utc).AddTicks(4029) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(139), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(140) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(152), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(153) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(160), new DateTime(2025, 10, 12, 5, 21, 42, 629, DateTimeKind.Utc).AddTicks(160) });
		}
	}
}
