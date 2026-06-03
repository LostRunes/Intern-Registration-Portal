using Microsoft.EntityFrameworkCore;
using InternPortal.Models;

namespace InternPortal.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Intern> Interns { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
}
