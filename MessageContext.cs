using MessageHandlingApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MessageHandlingApi
{
    public class MessageContext: DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Message> Message  { get; set; }
        public DbSet<Groups> Groups  { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=CEN_KGOTHATSOD1\SQLEXPRESS;Initial Catalog=MessageHandling;Integrated Security=True;MultipleActiveResultSets=True");
        }
    }
}