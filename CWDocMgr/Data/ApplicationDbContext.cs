using CWDocMgr.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CWDocMgr.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IConfiguration _settings;
        public DbSet<DocumentModel> Documents { get; set; }

        public ApplicationDbContext(IConfiguration settings, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _settings = settings;
        }
    }
}
