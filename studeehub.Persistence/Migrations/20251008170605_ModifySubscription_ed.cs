using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ModifySubscription_ed : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "UpdateAt",
				table: "Subscriptions",
				newName: "UpdatedAt");

			migrationBuilder.RenameColumn(
				name: "CreateAt",
				table: "Subscriptions",
				newName: "CreatedAt");

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

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "UpdatedAt",
				table: "Subscriptions",
				newName: "UpdateAt");

			migrationBuilder.RenameColumn(
				name: "CreatedAt",
				table: "Subscriptions",
				newName: "CreateAt");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAECXc+6T3A3J0HUVdSx92+a7tlUh2CvndBPhXMirxW1lRbQH9YgTjpttZDksyu6AX+Q==", new DateTime(2025, 10, 15, 16, 53, 36, 136, DateTimeKind.Utc).AddTicks(3682) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEOYyc32NoMQG3+5GYCMGkyJXwdPQwI+ps36vqGYiqK65mjRhsAfL+J9cuE+rdmL6Pg==", new DateTime(2025, 10, 15, 16, 53, 36, 89, DateTimeKind.Utc).AddTicks(718) });
		}
	}
}
