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
        public DbSet<Chat> Chat { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>()
                        .HasMany<Chat>(s => s.Courses)
                        .WithMany(c => c.Students)
                        .Map(cs =>
                                {
                                    cs.MapLeftKey("StudentRefId");
                                    cs.MapRightKey("CourseRefId");
                                    cs.ToTable("StudentCourse");
                                });

        }*/

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "\\appsettings.json");
            IConfigurationRoot config = builder.Build();

            optionsBuilder.UseSqlServer(config["ConnectionString"]);
        }
    }
}