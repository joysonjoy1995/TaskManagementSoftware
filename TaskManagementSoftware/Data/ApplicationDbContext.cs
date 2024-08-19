using Microsoft.EntityFrameworkCore;
using TaskManagementSoftware.Models;

namespace TaskManagementSoftware.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<TaskMaterialUsage> TaskMaterialUsages { get; set; }

        public async System.Threading.Tasks.Task ClearAllDataAsync()
        {
            TaskMaterialUsages.RemoveRange(TaskMaterialUsages);

            Tasks.RemoveRange(Tasks);

            Materials.RemoveRange(Materials);

            await SaveChangesAsync();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskMaterialUsage>()
                .HasKey(tmu => new { tmu.TaskID, tmu.MaterialID });

            modelBuilder.Entity<TaskMaterialUsage>()
                .HasOne(tmu => tmu.Task)
                .WithMany(t => t.TaskMaterialUsages)
                .HasForeignKey(tmu => tmu.TaskID);

            modelBuilder.Entity<TaskMaterialUsage>()
                .HasOne(tmu => tmu.Material)
                .WithMany(m => m.TaskMaterialUsages)
                .HasForeignKey(tmu => tmu.MaterialID);

            modelBuilder.Entity<Material>()
                .Property(m => m.UnitOfIssue)
                .HasConversion(
                    v => v.ToString(),
                    v => (Unit)Enum.Parse(typeof(Unit), v));

            modelBuilder.Entity<TaskMaterialUsage>()
                .Property(tmu => tmu.UnitOfMeasurement)
                .HasConversion(
                    v => v.ToString(),
                    v => (Unit)Enum.Parse(typeof(Unit), v));

            modelBuilder.Entity<Material>().HasData(
                new Material
                {
                    ID = Guid.NewGuid(),
                    PartNumber = "M001",
                    ManufacturerCode = 123,
                    Price = 50,
                    UnitOfIssue = Unit.Liter
                }
            );
        }
    }
}
