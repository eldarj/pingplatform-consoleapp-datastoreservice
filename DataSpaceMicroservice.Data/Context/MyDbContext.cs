using DataSpaceMicroservice.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Context
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> dbContextOptions) : base(dbContextOptions) { }

        #region DbSets
        public DbSet<Account> Accounts { get; set; }
        public DbSet<DSNode> DSNodes { get; set; }
        public DbSet<DSFile> DSFiles { get; set; }
        public DbSet<DSDirectory> DSDirectories { get; set; }
        public DbSet<FileAccountShare> FileAccountShares { get; set; }
        #endregion

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // TODO: Add this in appsettings or ENV (dev, prod) vars
        //    optionsBuilder.UseMySql("server=localhost;database=PingDataSpaceMicroserviceDb;user=root;password=",
        //        a => a.MigrationsAssembly("DataSpaceMicroservice.Data"));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<DSNode>()
                .Property(node => node.NodeType)
                .HasConversion<string>();

            //modelBuilder.Entity<DSFile>()
            //    .HasOne<Account>(file => file.Owner)
            //    .WithMany(owner => owner.OwningFiles)
            //    .HasForeignKey(file => file.OwnerId);

            //modelBuilder.Entity<DSDirectory>()
            //    .HasOne<Account>(dir => dir.Owner)
            //    .WithMany(owner => owner.OwningDirectories)
            //    .HasForeignKey(dir => dir.OwnerId);
        }
    }
}
