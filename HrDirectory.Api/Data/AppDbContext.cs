using Microsoft.EntityFrameworkCore;

namespace HrDirectory.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

}