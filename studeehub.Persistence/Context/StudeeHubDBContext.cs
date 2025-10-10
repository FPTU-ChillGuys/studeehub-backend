using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums.Achievements;

namespace studeehub.Persistence.Context
{
	public partial class StudeeHubDBContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public StudeeHubDBContext(DbContextOptions<StudeeHubDBContext> options)
			: base(options)
		{
		}

		// DbSets for domain entities (updated)
		public virtual DbSet<Workspace> Workspaces { get; set; } = null!;
		public virtual DbSet<Document> Documents { get; set; } = null!;
		public virtual DbSet<Flashcard> Flashcards { get; set; } = null!;
		public virtual DbSet<Note> Notes { get; set; } = null!;
		public virtual DbSet<Schedule> Schedules { get; set; } = null!;
		public virtual DbSet<Subscription> Subscriptions { get; set; } = null!;
		public virtual DbSet<Achievement> Achievements { get; set; } = null!;
		public virtual DbSet<UserAchievement> UserAchievements { get; set; } = null!;
		public virtual DbSet<PomodoroSession> PomodoroSessions { get; set; } = null!;
		public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
		public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
		public virtual DbSet<Streak> Streaks { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Workspace -- User (two-sided)
			modelBuilder.Entity<Workspace>()
				.HasKey(ws => ws.Id);

			modelBuilder.Entity<Workspace>()
				.HasOne(ws => ws.User)
				.WithMany(u => u.Workspaces)
				.HasForeignKey(ws => ws.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Document -- User
			modelBuilder.Entity<Document>()
				.HasKey(d => d.Id);

			modelBuilder.Entity<Document>()
				.HasOne(d => d.User)
				.WithMany()
				// changed to Restrict to avoid multiple cascade paths (User -> Documents and User -> Workspaces -> Documents)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Document -- Workspace (two-sided)
			modelBuilder.Entity<Document>()
				.HasOne(d => d.Workspace)
				.WithMany(ws => ws.Documents)
				.HasForeignKey(d => d.WorkSpaceId)
				.OnDelete(DeleteBehavior.Cascade);

			// Note -- User
			modelBuilder.Entity<Note>()
				.HasKey(n => n.Id);

			//// json constraint on Content
			//modelBuilder.Entity<Note>(entity =>
			//{
			//	entity.Property(e => e.Content)
			//		  .HasColumnType("NVARCHAR(MAX)");

			//	entity.ToTable(t => t.HasCheckConstraint("CK_Note_Content_IsJson", "ISJSON(Content) = 1"));
			//});
			modelBuilder.Entity<Note>(entity =>
			{
				entity.Property(e => e.Content)
					  .HasColumnType("NVARCHAR(MAX)");
			});

			modelBuilder.Entity<Note>()
				.HasOne(n => n.User)
				.WithMany()
				// changed to Restrict
				.HasForeignKey(n => n.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Note -- Workspace (two-sided)
			modelBuilder.Entity<Note>()
				.HasOne(n => n.Workspace)
				.WithMany(ws => ws.Notes)
				.HasForeignKey(n => n.WorkSpaceId)
				.OnDelete(DeleteBehavior.Cascade);

			// Flashcard -- User
			modelBuilder.Entity<Flashcard>()
				.HasKey(f => f.Id);

			modelBuilder.Entity<Flashcard>()
				.HasOne(f => f.User)
				.WithMany()
				// changed to Restrict
				.HasForeignKey(f => f.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Flashcard -- Workspace (two-sided)
			modelBuilder.Entity<Flashcard>()
				.HasOne(f => f.Workspace)
				.WithMany(ws => ws.Flashcards)
				.HasForeignKey(f => f.WorkSpaceId)
				.OnDelete(DeleteBehavior.Cascade);

			// Subscription -- User (two-sided)
			modelBuilder.Entity<Subscription>()
				.HasKey(s => s.Id);

			modelBuilder.Entity<Subscription>()
				.HasOne(s => s.User)
				.WithMany(u => u.Subscriptions)
				.HasForeignKey(s => s.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Subscription -> SubscriptionPlan (many subscriptions belong to a plan)
			modelBuilder.Entity<Subscription>()
				.HasOne(s => s.SubscriptionPlan)
				.WithMany(sp => sp.Subscriptions)
				.HasForeignKey(s => s.SubscriptionPlanId)
				.OnDelete(DeleteBehavior.Restrict);

			// Subscription -> PaymentTransactions (one-to-many)
			modelBuilder.Entity<Subscription>()
				.HasMany(s => s.PaymentTransactions)
				.WithOne(pt => pt.Subscription)
				.HasForeignKey(pt => pt.SubscriptionId)
				.OnDelete(DeleteBehavior.Cascade);

			// SubscriptionPlan (catalog)
			modelBuilder.Entity<SubscriptionPlan>()
				.HasKey(sp => sp.Id);

			// enforce unique plan codes
			modelBuilder.Entity<SubscriptionPlan>()
				.HasIndex(sp => sp.Code)
				.IsUnique();

			// prefer explicit precision for money
			modelBuilder.Entity<SubscriptionPlan>()
				.Property(sp => sp.Price)
				.HasColumnType("decimal(18,2)");

			// PaymentTransaction entity and relationships
			modelBuilder.Entity<PaymentTransaction>()
				.HasKey(pt => pt.Id);

			modelBuilder.Entity<PaymentTransaction>()
				.Property(pt => pt.TransactionCode)
				.IsUnicode();

			// Explicit precision/scale for Amount to avoid truncation warnings
			modelBuilder.Entity<PaymentTransaction>()
				.Property(pt => pt.Amount)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<PaymentTransaction>()
				.HasOne(pt => pt.User)
				.WithMany()
				.HasForeignKey(pt => pt.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PaymentTransaction>()
				.HasOne(pt => pt.Subscription)
				.WithMany(s => s.PaymentTransactions)
				.HasForeignKey(pt => pt.SubscriptionId)
				.OnDelete(DeleteBehavior.Cascade);

			// Achievement (catalog)
			modelBuilder.Entity<Achievement>()
				.HasKey(a => a.Id);

			// enforce unique achievement codes
			modelBuilder.Entity<Achievement>()
				.HasIndex(a => a.Code)
				.IsUnique();

			// PomodoroSession -- User (two-sided)
			modelBuilder.Entity<PomodoroSession>()
				.HasKey(p => p.Id);

			modelBuilder.Entity<PomodoroSession>()
				.HasOne(p => p.User)
				.WithMany(u => u.PomodoroSessions)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Streak -- User (two-sided)
			modelBuilder.Entity<Streak>()
				.HasKey(s => s.Id);

			modelBuilder.Entity<Streak>()
				.HasOne(s => s.User)
				.WithMany(u => u.Streaks)
				.HasForeignKey(s => s.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Schedule -- User (two-sided)
			modelBuilder.Entity<Schedule>()
				.HasKey(s => s.Id);

			modelBuilder.Entity<Schedule>()
				.HasOne(s => s.User)
				.WithMany(u => u.Schedules)
				.HasForeignKey(s => s.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// UserAchievement (join)
			modelBuilder.Entity<UserAchievement>()
				.HasKey(ua => new { ua.UserId, ua.AchievementId });

			modelBuilder.Entity<UserAchievement>()
				.HasOne(ua => ua.User)
				.WithMany(u => u.UserAchievements)
				.HasForeignKey(ua => ua.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<UserAchievement>()
				.HasOne(ua => ua.Achievement)
				.WithMany(a => a.UserAchievements)
				.HasForeignKey(ua => ua.AchievementId)
				.OnDelete(DeleteBehavior.Cascade);

			// Enums stored as strings (if used)
			modelBuilder.Entity<Streak>()
				.Property(s => s.Type)
				.HasConversion<string>();
			modelBuilder.Entity<Subscription>()
				.Property(s => s.Status)
				.HasConversion<string>();
			modelBuilder.Entity<Achievement>()
				.Property(a => a.ConditionType)
				.HasConversion<string>();
			modelBuilder.Entity<Achievement>()
				.Property(a => a.RewardType)
				.HasConversion<string>();
			modelBuilder.Entity<PaymentTransaction>()
				.Property(pt => pt.Status)
				.HasConversion<string>();

			// Seed lookup data: roles, users, subscription plans, achievements, user-achievements
			OnModelCreatingPartial(modelBuilder);

			modelBuilder.Entity<IdentityRole<Guid>>().HasData(SeedingRoles());
			modelBuilder.Entity<User>().HasData(SeedingUsers());
			modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(SeedingUserRoles());

			// Add subscription plans seed
			modelBuilder.Entity<SubscriptionPlan>().HasData(SeedingSubscriptionPlans());

			modelBuilder.Entity<Achievement>().HasData(SeedingAchievements());
			modelBuilder.Entity<UserAchievement>().HasData(SeedingUserAchievements());
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

		private ICollection<IdentityRole<Guid>> SeedingRoles()
		{
			return new List<IdentityRole<Guid>>()
			{
				new IdentityRole<Guid>
				{
					Id = Guid.Parse("3631e38b-60dd-4d1a-af7f-a26f21c2ef82"),
					Name = "admin",
					NormalizedName = "ADMIN",
					ConcurrencyStamp = "seed-1"
				},
				new IdentityRole<Guid>
				{
					Id = Guid.Parse("51ef7e08-ff07-459b-8c55-c7ebac505103"),
					Name = "user",
					NormalizedName = "USER",
					ConcurrencyStamp = "seed-2"
				}
			};
		}

		private ICollection<User> SeedingUsers()
		{
			var hasher = new PasswordHasher<User>();

			var admin = new User
			{
				Id = Guid.Parse("33f41895-b601-4aa1-8dc4-8229a9d07008"),
				UserName = "admin",
				NormalizedUserName = "ADMIN",
				Email = "admin@example.com",
				NormalizedEmail = "ADMIN@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-4",
				ConcurrencyStamp = "seed-5",
				CreatedAt = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc),
				UpdatedAt = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc)
			};
			admin.PasswordHash = hasher.HashPassword(admin, "12345aA@");

			var user = new User
			{
				Id = Guid.Parse("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
				UserName = "user",
				NormalizedUserName = "USER",
				Email = "user@example.com",
				NormalizedEmail = "USER@EXAMPLE.COM",
				EmailConfirmed = true,
				SecurityStamp = "seed-6",
				ConcurrencyStamp = "seed-7",
				CreatedAt = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc),
				UpdatedAt = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc)
			};
			user.PasswordHash = hasher.HashPassword(user, "12345aA@");

			return new List<User> { admin, user };
		}

		private ICollection<IdentityUserRole<Guid>> SeedingUserRoles()
		{
			return new List<IdentityUserRole<Guid>>
			{
				new IdentityUserRole<Guid>
				{
					UserId = Guid.Parse("33f41895-b601-4aa1-8dc4-8229a9d07008"),
					RoleId = Guid.Parse("3631e38b-60dd-4d1a-af7f-a26f21c2ef82")
				},
				new IdentityUserRole<Guid>
				{
					UserId = Guid.Parse("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
					RoleId = Guid.Parse("51ef7e08-ff07-459b-8c55-c7ebac505103")
				}
			};
		}

		// Seed some common subscription plans
		private ICollection<SubscriptionPlan> SeedingSubscriptionPlans()
		{
			return new List<SubscriptionPlan>
			{
				new SubscriptionPlan
				{
					Id = Guid.Parse("d2f1c3a4-1001-4a1b-8c1d-000000000101"),
					Code = "BASIC_MONTHLY",
					Name = "Gói Cơ Bản (Theo Tháng)",
					Description = "Gói cơ bản theo tháng với dung lượng và số lượng tài liệu giới hạn.",
					Price = 0m,
					Currency = "VND",
					DurationInDays = 30,
					IsActive = true,
					MaxDocuments = 50,
					MaxStorageMB = 500,
					HasAIAnalysis = false
				},
				new SubscriptionPlan
				{
					Id = Guid.Parse("d2f1c3a4-1002-4a1b-8c1d-000000000102"),
					Code = "PRO_MONTHLY",
					Name = "Gói Chuyên Nghiệp (Theo Tháng)",
					Description = "Gói chuyên nghiệp theo tháng với giới hạn cao hơn và có tính năng AI.",
					Price = 49.9m,
					Currency = "VND",
					DurationInDays = 30,
					IsActive = true,
					MaxDocuments = 1000,
					MaxStorageMB = 10240,
					HasAIAnalysis = true
				},
				new SubscriptionPlan
				{
					Id = Guid.Parse("d2f1c3a4-1003-4a1b-8c1d-000000000103"),
					Code = "PRO_YEARLY",
					Name = "Gói Chuyên Nghiệp (Theo Năm)",
					Description = "Gói chuyên nghiệp theo năm với mức giá ưu đãi khi thanh toán hàng năm.",
					Price = 499.99m,
					Currency = "VND",
					DurationInDays = 365,
					IsActive = true,
					MaxDocuments = 1000,
					MaxStorageMB = 10240,
					HasAIAnalysis = true
				}
			};
		}

		// Seed some common achievements for tests
		private ICollection<Achievement> SeedingAchievements()
		{
			return new List<Achievement>
			{
				new Achievement
				{
					Id = Guid.Parse("a1f2c3d4-0001-4a1b-8c1d-000000000001"),
					Code = "STREAK_3_DAYS",
					Name = "3-Day Streak",
					Description = "Maintain a 3-day streak.",
					ConditionValue = 3,
					ConditionType = ConditionType.Streak,
					RewardType = RewardType.XP,
					RewardValue = 50,
					IsActive = true
				},
				new Achievement
				{
					Id = Guid.Parse("a1f2c3d4-0002-4a1b-8c1d-000000000002"),
					Code = "STREAK_7_DAYS",
					Name = "7-Day Streak",
					Description = "Maintain a 7-day streak.",
					ConditionValue = 7,
					ConditionType = ConditionType.Streak,
					RewardType = RewardType.XP,
					RewardValue = 150,
					IsActive = true
				},
				new Achievement
				{
					Id = Guid.Parse("a1f2c3d4-0003-4a1b-8c1d-000000000003"),
					Code = "FIRST_NOTE",
					Name = "First Note Created",
					Description = "Create your first note.",
					ConditionValue = 1,
					ConditionType = ConditionType.NoteCreated,
					RewardType = RewardType.Badge,
					RewardValue = 1,
					IsActive = true
				},
				new Achievement
				{
					Id = Guid.Parse("a1f2c3d4-0004-4a1b-8c1d-000000000004"),
					Code = "FLASHCARDS_REVIEWED_10",
					Name = "10 Flashcards Reviewed",
					Description = "Review 10 flashcards.",
					ConditionValue = 10,
					ConditionType = ConditionType.FlashcardReviewed,
					RewardType = RewardType.XP,
					RewardValue = 100,
					IsActive = true
				},
				new Achievement
				{
					Id = Guid.Parse("a1f2c3d4-0005-4a1b-8c1d-000000000005"),
					Code = "DOCUMENTS_UPLOADED_10",
					Name = "10 Documents Uploaded",
					Description = "Upload 10 documents.",
					ConditionValue = 10,
					ConditionType = ConditionType.DocumentUpload,
					RewardType = RewardType.XP,
					RewardValue = 200,
					IsActive = true
				}
			};
		}

		// Seed user achievement links for test user
		private ICollection<UserAchievement> SeedingUserAchievements()
		{
			return new List<UserAchievement>
			{
				new UserAchievement
				{
					UserId = Guid.Parse("09097277-2705-40c2-bce5-51dbd1f4c1e6"), // seeded "user"
					AchievementId = Guid.Parse("a1f2c3d4-0001-4a1b-8c1d-000000000001"), // STREAK_3_DAYS
					UnlockedAt = new DateTime(2025, 10, 05, 0, 0, 0, DateTimeKind.Utc),
					IsClaimed = false
				},
				new UserAchievement
				{
					UserId = Guid.Parse("09097277-2705-40c2-bce5-51dbd1f4c1e6"),
					AchievementId = Guid.Parse("a1f2c3d4-0003-4a1b-8c1d-000000000003"), // FIRST_NOTE
					UnlockedAt = new DateTime(2025, 10, 04, 0, 0, 0, DateTimeKind.Utc),
					IsClaimed = true
				}
			};
		}
	}
}
