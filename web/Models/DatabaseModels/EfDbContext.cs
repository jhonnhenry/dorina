using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace web.Models.DatabaseModels
{
    public class EfDbContext : IdentityDbContext
    {
        public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options)
        { }
        public DbSet<ReceivedFile> ReceivedFiles { get; set; }
    }
}
