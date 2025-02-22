using DocMgrLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Runtime;


namespace DocMgrLib.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _settings;

        public DbSet<UserModel> Users { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }

        public ApplicationDbContext(IConfiguration settings)
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string connectionString = _settings.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    string sqLiteDbPath = _settings["SQLiteDbPath"];
        //    options.UseSqlite($"Data Source={sqLiteDbPath}");
        //}

    }
}
