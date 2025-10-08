using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ModifySubscription_addIsExpiredNotifi : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsExpiryNotified",
				table: "Subscriptions",
				type: "bit",
				nullable: false,
				defaultValue: false);

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

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsExpiryNotified",
				table: "Subscriptions");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEMQjUDz9lYTgHilJ1WHxpoJgKgMBope5DiuIUqX9DiFIOZodD0XCwRXC0z6MUePHJQ==", new DateTime(2025, 10, 15, 15, 27, 41, 599, DateTimeKind.Utc).AddTicks(317) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEHeNyu/kC9r84DMy7uqwKUvIAGRJIKsmHJ+fpIarQ13Oqk2/4Ub86tGAcUwUpTxy9w==", new DateTime(2025, 10, 15, 15, 27, 41, 551, DateTimeKind.Utc).AddTicks(8285) });
		}
	}
}
