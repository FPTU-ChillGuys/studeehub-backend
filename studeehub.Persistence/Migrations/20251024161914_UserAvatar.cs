using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 16, 19, 14, 348, DateTimeKind.Utc).AddTicks(5166), new DateTime(2025, 10, 24, 16, 19, 14, 348, DateTimeKind.Utc).AddTicks(5166) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 16, 19, 14, 348, DateTimeKind.Utc).AddTicks(5180), new DateTime(2025, 10, 24, 16, 19, 14, 348, DateTimeKind.Utc).AddTicks(5180) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
                columns: new[] { "PasswordHash", "ProfilePictureUrl", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEDoVTAtzfR35ZAKiGC2nME8F6of9EUNZi/svIu43wxNATSgsg9OIjGL9mP5KkTbSDA==", "", new DateTime(2025, 10, 31, 16, 19, 14, 299, DateTimeKind.Utc).AddTicks(9654) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                columns: new[] { "PasswordHash", "ProfilePictureUrl", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAELD+j54gVmBsUQulW6tcQTFdrjppZk6O0WTniR/jHRc6pvsFhfhVztgNOzDhwA1cOA==", "", new DateTime(2025, 10, 31, 16, 19, 14, 252, DateTimeKind.Utc).AddTicks(4580) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3042), new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3042) });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3051), new DateTime(2025, 10, 24, 15, 15, 24, 71, DateTimeKind.Utc).AddTicks(3051) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAECmM8+Cexrcr1wecnYQR8UeeyUY99EHADYuzAilYT15yzk59sRqFZuVhN3pomLTNPQ==", new DateTime(2025, 10, 31, 15, 15, 24, 22, DateTimeKind.Utc).AddTicks(6757) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEKtb/poWWXjseAxA7ErbghrjWATO4aEwbD5rbriumnYjueaWSUn5RKmgQKsILS7JzQ==", new DateTime(2025, 10, 31, 15, 15, 23, 975, DateTimeKind.Utc).AddTicks(1147) });
        }
    }
}
