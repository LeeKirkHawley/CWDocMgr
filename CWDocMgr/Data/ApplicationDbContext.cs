﻿using CWDocMgr.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string sqLiteDbPath = _settings["SQLiteDbPath"];
            options.UseSqlite($"Data Source={sqLiteDbPath}");
        }

    }
}
