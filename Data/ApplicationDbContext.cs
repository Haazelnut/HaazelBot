using Haazelbot.Guilds;
using Haazelbot.UserAccounts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haazelbot.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserAccount> Accounts { get; set; }
        public DbSet<Guild> Guilds { get; set; }

        public ApplicationDbContext()
        {

        }

        private const string connString = "Server=(localdb)\\mssqllocaldb;Database=HaazelBot;Trusted_Connection=True;MultipleActiveResultSets=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(connString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }
    }
}
