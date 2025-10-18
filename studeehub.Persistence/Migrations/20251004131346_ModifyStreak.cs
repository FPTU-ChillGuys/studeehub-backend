using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ModifyStreak : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Type",
				table: "Streaks",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEIJ5WqZm/mz52tsaffPu6/3Xr2qYSq8bAS6/L2WAMBKIXtze3YFP533Hx8MDXWTSpA==", new DateTime(2025, 10, 11, 13, 13, 45, 270, DateTimeKind.Utc).AddTicks(1747) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEA5aVewqAvFaAAnQMACsUzFPftXNR7V2wkaY3OsmT2wTMRcl5zwT9CwiXPzKAYY6og==", new DateTime(2025, 10, 11, 13, 13, 45, 222, DateTimeKind.Utc).AddTicks(5918) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Type",
				table: "Streaks");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEC6PLuRwISeKWANdp/1ViTDVWBHCCjqKWV7HjCL/XZI1mmR+S2FHREQl86MksTvEJQ==", new DateTime(2025, 10, 10, 16, 6, 50, 38, DateTimeKind.Utc).AddTicks(7976) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEN1zqmYRr9GHphQdhJyuXRsJZhAd/i8BgIAv0u/6mYXDbtgVBO8CN9u23/tKDhe+Aw==", new DateTime(2025, 10, 10, 16, 6, 49, 990, DateTimeKind.Utc).AddTicks(6185) });
		}
	}
}
