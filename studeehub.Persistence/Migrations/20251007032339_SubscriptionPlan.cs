using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class SubscriptionPlan : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Type",
				table: "Subscriptions");

			migrationBuilder.AddColumn<Guid>(
				name: "SubscriptionPlanId",
				table: "Subscriptions",
				type: "uniqueidentifier",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			migrationBuilder.CreateTable(
				name: "PaymentTransactions",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
					TransactionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
					BankCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
					table.ForeignKey(
						name: "FK_PaymentTransactions_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_PaymentTransactions_Subscriptions_SubscriptionId",
						column: x => x.SubscriptionId,
						principalTable: "Subscriptions",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "SubscriptionPlans",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
					DurationInDays = table.Column<int>(type: "int", nullable: false),
					IsActive = table.Column<bool>(type: "bit", nullable: false),
					MaxDocuments = table.Column<int>(type: "int", nullable: false),
					MaxStorageMB = table.Column<int>(type: "int", nullable: false),
					HasAIAnalysis = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
				});

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

			migrationBuilder.CreateIndex(
				name: "IX_Subscriptions_SubscriptionPlanId",
				table: "Subscriptions",
				column: "SubscriptionPlanId");

			migrationBuilder.CreateIndex(
				name: "IX_PaymentTransactions_SubscriptionId",
				table: "PaymentTransactions",
				column: "SubscriptionId");

			migrationBuilder.CreateIndex(
				name: "IX_PaymentTransactions_UserId",
				table: "PaymentTransactions",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_SubscriptionPlans_Code",
				table: "SubscriptionPlans",
				column: "Code",
				unique: true);

			migrationBuilder.AddForeignKey(
				name: "FK_Subscriptions_SubscriptionPlans_SubscriptionPlanId",
				table: "Subscriptions",
				column: "SubscriptionPlanId",
				principalTable: "SubscriptionPlans",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Subscriptions_SubscriptionPlans_SubscriptionPlanId",
				table: "Subscriptions");

			migrationBuilder.DropTable(
				name: "PaymentTransactions");

			migrationBuilder.DropTable(
				name: "SubscriptionPlans");

			migrationBuilder.DropIndex(
				name: "IX_Subscriptions_SubscriptionPlanId",
				table: "Subscriptions");

			migrationBuilder.DropColumn(
				name: "SubscriptionPlanId",
				table: "Subscriptions");

			migrationBuilder.AddColumn<string>(
				name: "Type",
				table: "Subscriptions",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEHUrXxq/cFvDcyMlcpODUfPsIP9HEnyIKVtd6b5u+2Vla6mnWi+VWf1oa2/VKCbvcA==", new DateTime(2025, 10, 13, 8, 38, 55, 286, DateTimeKind.Utc).AddTicks(3515) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEIqqLUps53vz8qfy1T6f0VgrO+sHn1Kj7ldSa+0XSU4GjqU7FaTOMCnV2A0kZapp4w==", new DateTime(2025, 10, 13, 8, 38, 55, 236, DateTimeKind.Utc).AddTicks(3264) });
		}
	}
}
