using JobBoard.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobBoard.Context;
public class JobBoardDbContext(DbContextOptions<JobBoardDbContext> options) : IdentityDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public DbSet<Applicant> Applicants { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Job> Jobs { get; set; }
}
