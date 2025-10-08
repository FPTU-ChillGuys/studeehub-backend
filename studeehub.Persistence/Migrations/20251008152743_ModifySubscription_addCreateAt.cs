using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifySubscription_addCreateAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Subscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Subscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Subscriptions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEF25bX7+3aKMDCRXwPH1L7T7D2izxlnMYjTHbm3OV4V1I1yqo/MtSdgVzZXjU3sOWQ==", new DateTime(2025, 10, 14, 15, 8, 54, 756, DateTimeKind.Utc).AddTicks(1452) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEArNEpvEBhKIdI30tk9GG2Vfo30M4Jcr/6L5tIGhh1fn3V0mYplGWlgk8mufaHbDFw==", new DateTime(2025, 10, 14, 15, 8, 54, 705, DateTimeKind.Utc).AddTicks(625) });
        }
    }
}
