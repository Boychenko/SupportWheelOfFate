using Microsoft.EntityFrameworkCore;
using SupportWheelOfFate.Domain.Models;

namespace SupportWheelOfFate.Database
{
    public class SupportWheelContext : DbContext
    {
        public SupportWheelContext(DbContextOptions<SupportWheelContext> options) : base(options)
        {
        }

        public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
        public DbSet<Engineer> Engineers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupKey<ScheduleEntry>(modelBuilder);
            modelBuilder.Entity<ScheduleEntry>()
                .HasOne(s => s.Engineer)
                .WithMany()
                .HasForeignKey(s => s.EngineerId);

            SetupKey<Engineer>(modelBuilder);
            modelBuilder.Entity<Engineer>().Property(e => e.Name).IsRequired();
            modelBuilder.Entity<Engineer>().Property(e => e.Name).HasMaxLength(500);
        }

        private void SetupKey<T>(ModelBuilder modelBuilder) where T: Entity
        {
            modelBuilder.Entity<T>().HasKey(s => s.Id);
            modelBuilder.Entity<T>().Property(s => s.Id).ValueGeneratedOnAdd();
        }
    }
}
