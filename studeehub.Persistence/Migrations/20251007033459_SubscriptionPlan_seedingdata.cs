using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class SubscriptionPlan_seedingdata : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
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

			migrationBuilder.InsertData(
				table: "SubscriptionPlans",
				columns: new[] { "Id", "Code", "Currency", "Description", "DurationInDays", "HasAIAnalysis", "IsActive", "MaxDocuments", "MaxStorageMB", "Name", "Price" },
				values: new object[,]
				{
					{ new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"), "BASIC_MONTHLY", "VND", "Gói cơ bản theo tháng với dung lượng và số lượng tài liệu giới hạn.", 30, false, true, 50, 500, "Gói Cơ Bản (Theo Tháng)", 0m },
					{ new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"), "PRO_MONTHLY", "VND", "Gói chuyên nghiệp theo tháng với giới hạn cao hơn và có tính năng AI.", 30, true, true, 1000, 10240, "Gói Chuyên Nghiệp (Theo Tháng)", 49.9m },
					{ new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"), "PRO_YEARLY", "VND", "Gói chuyên nghiệp theo năm với mức giá ưu đãi khi thanh toán hàng năm.", 365, true, true, 1000, 10240, "Gói Chuyên Nghiệp (Theo Năm)", 499.99m }
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DeleteData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1001-4a1b-8c1d-000000000101"));

			migrationBuilder.DeleteData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1002-4a1b-8c1d-000000000102"));

			migrationBuilder.DeleteData(
				table: "SubscriptionPlans",
				keyColumn: "Id",
				keyValue: new Guid("d2f1c3a4-1003-4a1b-8c1d-000000000103"));

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAECJwSHwZVutU72z/aNgnPVBuCivv2LxAHmhcertjXzJUULdzwTnwKO5FAW9ALymkEA==", new DateTime(2025, 10, 14, 3, 23, 38, 817, DateTimeKind.Utc).AddTicks(7090) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEMfbcSn8imR14CEOX1QXhv8E4K+p3c7am3ftHiDKl2anT8EBsf3wNk4t2bEsiUMdxQ==", new DateTime(2025, 10, 14, 3, 23, 38, 768, DateTimeKind.Utc).AddTicks(7852) });
		}
	}
}
