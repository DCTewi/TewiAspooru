using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TewiBoard.Web.Models;

namespace TewiBoard.Web.Repositories
{
    public class TewiDbContext : DbContext
    {
        private IConfiguration Configuration { get; }

        public TewiDbContext(IConfiguration config)
        {
            Configuration = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Configuration.GetConnectionString("database"));
        }

        public DbSet<CardModel> Cards { get; set; }
    }
}
