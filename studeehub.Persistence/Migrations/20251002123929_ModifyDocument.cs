using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ModifyDocument : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "UpdatedAt",
				table: "Documents");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEAOR9s7WwVqk4rwkmHPtaDMh6sMCr9504ursKZqqV+gys7/4ph1shpRrCjx7l/CVxw==", new DateTime(2025, 10, 9, 12, 39, 28, 84, DateTimeKind.Utc).AddTicks(5579) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEHFJk2ymCfLO7wxn9/ryBruyGRvUaxN3EVJAuDRZ6x42z+y+UL2VkrixxL/G9LEQVg==", new DateTime(2025, 10, 9, 12, 39, 28, 34, DateTimeKind.Utc).AddTicks(9537) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "UpdatedAt",
				table: "Documents",
				type: "datetime2",
				nullable: true);

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
	}
}
