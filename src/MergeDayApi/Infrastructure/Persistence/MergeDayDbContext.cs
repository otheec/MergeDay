using MergeDayApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace MergeDayApi.Infrastructure.Persistence;

public class MergeDayDbContext(DbContextOptions<MergeDayDbContext> options) : DbContext(options)
{
    public DbSet<Absence> Absences { get; set; } = null!;
}
