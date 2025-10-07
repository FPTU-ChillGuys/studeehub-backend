using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTransaction_Remove_BankNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "PaymentTransactions");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEAtvGN4GcVe5xEXkB6r3wWNUdXMioFvSBGTLripZOZdV272oXc01ZWqc6YtfyJCTbw==", new DateTime(2025, 10, 14, 3, 34, 58, 809, DateTimeKind.Utc).AddTicks(3243) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "AQAAAAIAAYagAAAAEAk01bYu4iAvqhFAEESq33pWSx+XyW8p1FQlcmF6chE+SQVb3pPeD0sElea2womtMQ==", new DateTime(2025, 10, 14, 3, 34, 58, 759, DateTimeKind.Utc).AddTicks(957) });
        }
    }
}
