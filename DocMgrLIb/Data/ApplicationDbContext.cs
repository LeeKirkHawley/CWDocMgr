using DocMgrLib.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace DocMgrLib.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
