using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studeehub.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ModifyAchievement_ConditionType : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsCheckin",
				table: "Schedule",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "Code", "ConditionType", "Description", "Name" },
				values: new object[] { "FIRST_NOTE", "NoteCreated", "Create your first note.", "First Note Created" });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "Code", "ConditionType", "Description", "Name" },
				values: new object[] { "FLASHCARDS_REVIEWED_10", "FlashcardReviewed", "Review 10 flashcards.", "10 Flashcards Reviewed" });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "Code", "ConditionType", "Description", "Name" },
				values: new object[] { "DOCUMENTS_UPLOADED_10", "DocumentUpload", "Upload 10 documents.", "10 Documents Uploaded" });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEHm1ueb++Mkz9N1BcHcxyeRAzTnFBGOpN6F1rGiJshJVnMice1AZ25GVgeszf9maFA==", new DateTime(2025, 10, 12, 14, 56, 16, 706, DateTimeKind.Utc).AddTicks(8760) });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: new Guid("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				columns: new[] { "PasswordHash", "RefreshTokenExpiryTime" },
				values: new object[] { "AQAAAAIAAYagAAAAEJj3BEK0FUaeZjBLB+o3AVdXD/+yiau9ZCWLNjTYDv9qkQQ0B8u9I8yODTdNvjr9hQ==", new DateTime(2025, 10, 12, 14, 56, 16, 659, DateTimeKind.Utc).AddTicks(2117) });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsCheckin",
				table: "Schedule");

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
				columns: new[] { "Code", "ConditionType", "Description", "Name" },
				values: new object[] { "FIRST_QUIZ", "QuizCompleted", "Complete your first quiz.", "First Quiz Completed" });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
				columns: new[] { "Code", "ConditionType", "Description", "Name" },
				values: new object[] { "TASKS_10", "TasksCompleted", "Complete 10 tasks.", "10 Tasks Completed" });

			migrationBuilder.UpdateData(
				table: "Achievements",
				keyColumn: "Id",
				keyValue: new Guid("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
				columns: new[] { "Code", "ConditionType", "Description", "Name" },
				values: new object[] { "HOURS_10", "HoursStudied", "Accumulate 10 hours of study.", "10 Hours Studied" });

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
		}
	}
}
