using Microsoft.EntityFrameworkCore;
using StudyRoom.Models;

namespace StudyRoom.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<StudySession> StudySessions { get; set; } = null!;
        public DbSet<Participation> Participations { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudySession>()
                .HasOne(s => s.Room)
                .WithMany(r => r.StudySessions)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudySession>()
                .HasOne(s => s.Host)
                .WithMany(st => st.HostedSessions)
                .HasForeignKey(s => s.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Student)
                .WithMany(st => st.Participations)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Participation>()
                .HasOne(p => p.StudySession)
                .WithMany(s => s.Participations)
                .HasForeignKey(p => p.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Student)
                .WithMany(st => st.Messages)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.StudySession)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}