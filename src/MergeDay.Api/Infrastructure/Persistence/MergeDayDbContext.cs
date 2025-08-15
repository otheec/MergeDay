using MergeDay.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Infrastructure.Persistence;

public class MergeDayDbContext(DbContextOptions<MergeDayDbContext> options) : DbContext(options)
{
    public DbSet<Absence> Absences { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
}
