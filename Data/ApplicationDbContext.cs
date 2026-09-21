using Microsoft.EntityFrameworkCore;
using StudyRoom.Models;

namespace StudyRoom.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<StudySession> StudySessions => Set<StudySession>();
}
