using MessageHandlingApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MessageHandlingApi
{
    public class MessageContext: DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Message> Message  { get; set; }
        public DbSet<Groups> Groups  { get; set; }
        public DbSet<AccountGroup> AccountGroup  { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "\\appsettings.json");
            IConfigurationRoot config = builder.Build();

            optionsBuilder.UseSqlServer(config["ConnectionString"]);
        }
    }
}