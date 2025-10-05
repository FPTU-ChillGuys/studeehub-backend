using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class AddUserAchievement : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Achievements_AspNetUsers_UserId",
				table: "Achievements");

			migrationBuilder.DropIndex(
				name: "IX_Achievements_UserId",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "UnlockedAt",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "UserId",
				table: "Achievements");

			migrationBuilder.AddColumn<string>(
				name: "Code",
				table: "Achievements",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "ConditionType",
				table: "Achievements",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<int>(
				name: "ConditionValue",
				table: "Achievements",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<bool>(
				name: "IsActive",
				table: "Achievements",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<string>(
				name: "RewardType",
				table: "Achievements",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<int>(
				name: "RewardValue",
				table: "Achievements",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateTable(
				name: "UserAchievements",
				columns: table => new
				{
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AchievementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					UnlockedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					IsClaimed = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_UserAchievements", x => new { x.UserId, x.AchievementId });
					table.ForeignKey(
						name: "FK_UserAchievements_Achievements_AchievementId",
						column: x => x.AchievementId,
						principalTable: "Achievements",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_UserAchievements_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.InsertData(
				table: "Achievements",
				columns: new[] { "Id", "Code", "ConditionType", "ConditionValue", "Description", "IsActive", "Name", "RewardType", "RewardValue" },
				values: new object[,]
				{
					{ new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"), "STREAK_3_DAYS", "Streak", 3, "Maintain a 3-day streak.", true, "3-Day Streak", "XP", 50 },
					{ new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"), "STREAK_7_DAYS", "Streak", 7, "Maintain a 7-day streak.", true, "7-Day Streak", "XP", 150 },
					{ new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"), "FIRST_QUIZ", "QuizCompleted", 1, "Complete your first quiz.", true, "First Quiz Completed", "Badge", 1 },
					{ new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"), "TASKS_10", "TasksCompleted", 10, "Complete 10 tasks.", true, "10 Tasks Completed", "XP", 100 },
					{ new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"), "HOURS_10", "HoursStudied", 10, "Accumulate 10 hours of study.", true, "10 Hours Studied", "XP", 200 }
				});

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAECX36buyjwyIR7xyrck7CKSIRACokU5EIXevt0qLICA7zkbbQHrA/4CQRNNcGiRhFg==", new DateTime(2025, 10, 12, 8, 21, 1, 897, DateTimeKind.Utc).AddTicks(2450) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEC0MP9NOgbTx9ytGTZE5kipKHWObkkaMotkFysx55uJWJwQK9tLSZjrgpV6GvnhBnQ==", new DateTime(2025, 10, 12, 8, 21, 1, 849, DateTimeKind.Utc).AddTicks(3818) });

			migrationBuilder.InsertData(
				table: "UserAchievements",
				columns: new[] { "AchievementId", "UserId", "IsClaimed", "UnlockedAt" },
				values: new object[,]
				{
					{ new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"), new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"), false, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
					{ new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"), new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"), true, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
				});

			migrationBuilder.CreateIndex(
				name: "IX_UserAchievements_AchievementId",
				table: "UserAchievements",
				column: "AchievementId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "UserAchievements");

			migrationBuilder.DeleteData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0001-4a1b-8c1d-000000000001"));

			migrationBuilder.DeleteData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0002-4a1b-8c1d-000000000002"));

			migrationBuilder.DeleteData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"));

			migrationBuilder.DeleteData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"));

			migrationBuilder.DeleteData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"));

			migrationBuilder.DropColumn(
				name: "Code",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "ConditionType",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "ConditionValue",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "IsActive",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "RewardType",
				table: "Achievements");

			migrationBuilder.DropColumn(
				name: "RewardValue",
				table: "Achievements");

			migrationBuilder.AddColumn<DateTime>(
				name: "UnlockedAt",
				table: "Achievements",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<Guid>(
				name: "UserId",
				table: "Achievements",
				type: "uniqueidentifier",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

			migrationBuilder.CreateIndex(
				name: "IX_Achievements_UserId",
				table: "Achievements",
				column: "UserId");

			migrationBuilder.AddForeignKey(
				name: "FK_Achievements_AspNetUsers_UserId",
				table: "Achievements",
				column: "UserId",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
