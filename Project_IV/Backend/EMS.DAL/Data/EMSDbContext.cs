using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.DataAccessLayer.Data
{
    public class EMSDbContext : DbContext
    {
        public EMSDbContext(DbContextOptions<EMSDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserInfo> Users { get; set; }
        public DbSet<EventDetails> Events { get; set; }
        public DbSet<SessionInfo> Sessions { get; set; }
        public DbSet<SpeakersDetails> Speakers { get; set; }
        public DbSet<ParticipantEventDetails> ParticipantEvents { get; set; }
        public DbSet<CategoryDetails> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SessionInfo>()
                .HasOne(s => s.Event)
                .WithMany(e => e.Sessions)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SessionInfo>()
                .HasOne(s => s.Speaker)
                .WithMany()
                .HasForeignKey(s => s.SpeakerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ParticipantEventDetails>()
                .HasOne(p => p.Event)
                .WithMany()
                .HasForeignKey(p => p.EventId);

            modelBuilder.Entity<ParticipantEventDetails>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.ParticipantEmailId);

            modelBuilder.Entity<ParticipantEventDetails>()
                .HasIndex(p => new { p.ParticipantEmailId, p.EventId })
                .IsUnique()
                .HasDatabaseName("UX_ParticipantEvents_Email_Event");

            // CategoryDetails: unique name
            modelBuilder.Entity<CategoryDetails>()
                .HasIndex(c => c.CategoryName)
                .IsUnique()
                .HasDatabaseName("UX_Categories_Name");

            // Seed category data
            modelBuilder.Entity<CategoryDetails>().HasData(
                new CategoryDetails { Id = new Guid("318cb9e1-9795-4354-98a6-5bb8b5d5dbe5"), CategoryName = "Business" },
                new CategoryDetails { Id = new Guid("aabd70a7-92fb-4927-afba-d83a1427c131"), CategoryName = "Cloud & DevOps" },
                new CategoryDetails { Id = new Guid("a8ae7c75-b662-486f-b06e-08d7cbc3f65e"), CategoryName = "Design" },
                new CategoryDetails { Id = new Guid("291ccb40-4520-430b-bc85-528810c5ad33"), CategoryName = "Marketing" },
                new CategoryDetails { Id = new Guid("01875422-049f-4198-bd93-89e4bfe4ad4d"), CategoryName = "Technology" }
);

            // Seed default Admin user (BCrypt hash of "Admin@123")
            modelBuilder.Entity<UserInfo>().HasData(new UserInfo
            {
                EmailId  = "admin@ems.com",
                UserName = "Admin",
                Role     = "Admin",
                Password = "$2a$11$IAF00m.BFFGKVKi.eFn3YO43CJ7box7Gm2S8LOO6UWRVlpzKLUWa2"
            });
        }
    }
}
