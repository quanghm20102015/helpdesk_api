﻿using Microsoft.EntityFrameworkCore;

namespace HelpDeskSystem.Models
{
    public class EF_DataContext : DbContext
    {
        public EF_DataContext(DbContextOptions<EF_DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }
        public DbSet<Product> Products
        {
            get;
            set;
        }
        public DbSet<Order> Orders
        {
            get;
            set;
        }
        public DbSet<Account> Accounts
        {
            get;
            set;
        }
    }
}
