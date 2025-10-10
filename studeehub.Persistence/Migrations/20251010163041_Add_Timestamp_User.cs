using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class Add_Timestamp_User : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedAt",
				table: "AspNetUsers",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				name: "UpdatedAt",
				table: "AspNetUsers",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5692), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5692) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5704), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5704) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5708), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5708) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5710), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5710) });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5712), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5712) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "CreatedAt", "PasswordHash", "RefreshTokenExpiryTime", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEAiac8QUHqKQkmXvRhDovSu5bUHf5cLIG7BSnj+Z1pgjhL7pSpaqfgugC9pPHD9lDw==", new DateTime(2025, 10, 17, 16, 30, 39, 871, DateTimeKind.Utc).AddTicks(940), new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "CreatedAt", "PasswordHash", "RefreshTokenExpiryTime", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAELNq1kOvl772Gle81Oss9hSi/FwkZ5M+j2uM9jLIfgBy1OBtL/PSSinHB9tMCY7C0g==", new DateTime(2025, 10, 17, 16, 30, 39, 823, DateTimeKind.Utc).AddTicks(6628), new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5620), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5620) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5631), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5631) });

			migrationBuilder.UpdateData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
				columns: new[] { "CreatedAt", "UpdatedAt" },
				values: new object[] { new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5638), new DateTime(2025, 10, 10, 16, 30, 39, 918, DateTimeKind.Utc).AddTicks(5639) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "CreatedAt",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "UpdatedAt",
				table: "AspNetUsers");

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
	}
}
