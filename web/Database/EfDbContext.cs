using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using web.Database.DatabaseModels;

namespace web.Database
{
    public class EfDbContext : IdentityDbContext
    {
        public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options)
        { }
        public DbSet<ReceivedFile> ReceivedFiles { get; set; }
    }
}
