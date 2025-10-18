using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ModifyNote : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropCheckConstraint(
				name: "CK_Note_Content_IsJson",
				table: "Notes");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEDdVGvyRgjWxOpC6jSSFtly5EtcfOUoP8vBkn74IzrDFfW5AEM8Q6WXRBjc4DUfL1g==", new DateTime(2025, 10, 9, 11, 5, 21, 481, DateTimeKind.Utc).AddTicks(5320) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEIC5fQx/04hmBQWRG3MI609+Jdv8ht6zKnsLfA/4OUGIpIatGU8Zivhloi6PdvIR9g==", new DateTime(2025, 10, 9, 11, 5, 21, 431, DateTimeKind.Utc).AddTicks(8222) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEAuhHqpAULhaHJODep0CRnbKg9WhP3QybicCtGQerWqp4l5JzBb3rSr1W6IWKA61zw==", new DateTime(2025, 10, 9, 4, 14, 28, 887, DateTimeKind.Utc).AddTicks(9197) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAED+lTCLNTAJ9iWTDcjgv/7x8qPlW0pH2xzelXvaFXmOj577fV7c6TtVkmLDIYxdtgw==", new DateTime(2025, 10, 9, 4, 14, 28, 839, DateTimeKind.Utc).AddTicks(4387) });

			migrationBuilder.AddCheckConstraint(
				name: "CK_Note_Content_IsJson",
				table: "Notes",
				sql: "ISJSON(Content) = 1");
		}
	}
}
