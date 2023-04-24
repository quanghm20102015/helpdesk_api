using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Models;

namespace HelpDeskSystem.Models
{
    public class EF_DataContext : DbContext
    {
        public EF_DataContext(DbContextOptions<EF_DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }
        public DbSet<Account> Accounts
        {
            get;
            set;
        }
        public DbSet<Country> Countrys
        {
            get;
            set;
        }
        public DbSet<Contact> Contacts
        {
            get;
            set;
        }
        public DbSet<ConfigMail> ConfigMails
        {
            get;
            set;
        }
        public DbSet<EmailInfo> EmailInfos
        {
            get;
            set;
        }
        public DbSet<Company> Companys { get; set; }
    }
}
