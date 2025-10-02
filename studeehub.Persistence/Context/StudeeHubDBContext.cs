using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using studeehub.Domain.Entities;

namespace studeehub.Persistence.Context
{
	public partial class StudeeHubDBContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public StudeeHubDBContext(DbContextOptions<StudeeHubDBContext> options)
			: base(options)
		{
		}

		// DbSets for domain entities (updated)
		public virtual DbSet<WorkSpace> WorkSpaces { get; set; } = null!;
		public virtual DbSet<Document> Documents { get; set; } = null!;
		public virtual DbSet<Flashcard> Flashcards { get; set; } = null!;
		public virtual DbSet<Note> Notes { get; set; } = null!;
		public virtual DbSet<Subscription> Subscriptions { get; set; } = null!;
		public virtual DbSet<Achievement> Achievements { get; set; } = null!;
		public virtual DbSet<PomodoroSession> PomodoroSessions { get; set; } = null!;
		public virtual DbSet<Streak> Streaks { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// WorkSpace -- User (two-sided)
			modelBuilder.Entity<WorkSpace>()
				.HasKey(ws => ws.Id);

			modelBuilder.Entity<WorkSpace>()
				.HasOne(ws => ws.User)
				.WithMany(u => u.WorkSpaces)
				.HasForeignKey(ws => ws.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Document -- User
			modelBuilder.Entity<Document>()
				.HasKey(d => d.Id);

			modelBuilder.Entity<Document>()
				.HasOne(d => d.User)
				.WithMany()
				// changed to Restrict to avoid multiple cascade paths (User -> Documents and User -> WorkSpaces -> Documents)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Document -- WorkSpace (two-sided)
			modelBuilder.Entity<Document>()
				.HasOne(d => d.WorkSpace)
				.WithMany(ws => ws.Documents)
				.HasForeignKey(d => d.WorkSpaceId)
				.OnDelete(DeleteBehavior.Cascade);

			// Note -- User
			modelBuilder.Entity<Note>()
				.HasKey(n => n.Id);

			// json constraint on Content
			modelBuilder.Entity<Note>(entity =>
			{
				entity.Property(e => e.Content)
					  .HasColumnType("NVARCHAR(MAX)");

				entity.ToTable(t => t.HasCheckConstraint("CK_Note_Content_IsJson", "ISJSON(Content) = 1"));
			});

			modelBuilder.Entity<Note>()
				.HasOne(n => n.User)
				.WithMany()
				// changed to Restrict
				.HasForeignKey(n => n.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Note -- WorkSpace (two-sided)
			modelBuilder.Entity<Note>()
				.HasOne(n => n.WorkSpace)
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

			// Flashcard -- WorkSpace (two-sided)
			modelBuilder.Entity<Flashcard>()
				.HasOne(f => f.WorkSpace)
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

			// Achievement -- User (two-sided)
			modelBuilder.Entity<Achievement>()
				.HasKey(a => a.Id);

			modelBuilder.Entity<Achievement>()
				.HasOne(a => a.User)
				.WithMany(u => u.Achievements)
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.Cascade);

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

			// Enums stored as strings (if used)
			modelBuilder.Entity<Subscription>()
				.Property(s => s.Type)
				.HasConversion<string>();
			modelBuilder.Entity<Subscription>()
				.Property(s => s.Status)
				.HasConversion<string>();

			OnModelCreatingPartial(modelBuilder);

			modelBuilder.Entity<IdentityRole<Guid>>().HasData(SeedingRoles());
			modelBuilder.Entity<User>().HasData(SeedingUsers());
			modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(SeedingUserRoles());
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
				PasswordHash = hasher.HashPassword(null!, "12345aA@")
			};

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
				PasswordHash = hasher.HashPassword(null!, "12345aA@")
			};

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
	}
}
