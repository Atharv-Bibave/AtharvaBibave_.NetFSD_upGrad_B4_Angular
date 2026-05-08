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
                new CategoryDetails { Id = new Guid("11111111-0000-0000-0000-000000000001"), CategoryName = "Tech & Innovation" },
                new CategoryDetails { Id = new Guid("11111111-0000-0000-0000-000000000002"), CategoryName = "Industrial Event" },
                new CategoryDetails { Id = new Guid("11111111-0000-0000-0000-000000000003"), CategoryName = "Workshop" },
                new CategoryDetails { Id = new Guid("11111111-0000-0000-0000-000000000004"), CategoryName = "Solution and Projects" },
                new CategoryDetails { Id = new Guid("11111111-0000-0000-0000-000000000005"), CategoryName = "EMS (Solution)" }
            );
        }
    }
}
