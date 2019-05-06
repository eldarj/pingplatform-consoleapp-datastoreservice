using DataSpaceMicroservice.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Context
{
    public class MyDbContext : DbContext
    {

        #region DbSets
        public DbSet<Account> Accounts { get; set; }
        public DbSet<DSFile> Files { get; set; }
        public DbSet<DSDirectory> Directories { get; set; }
        public DbSet<FileAccountShare> FileAccountShares { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Add this in appsettings or ENV (dev, prod) vars
            optionsBuilder.UseMySql("server=localhost;database=PingDataSpaceMicroserviceDb;user=root;password=",
                a => a.MigrationsAssembly("DataSpaceMicroservice.Data"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.PhoneNumber)
                .IsUnique();
        }
    }
}
