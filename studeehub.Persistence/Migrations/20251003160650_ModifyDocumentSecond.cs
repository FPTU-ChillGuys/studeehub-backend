using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDocumentSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Documents",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Documents",
                newName: "Title");

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
    }
}
