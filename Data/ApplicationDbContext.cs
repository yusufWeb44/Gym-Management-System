using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;

namespace GymManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FitnessType> FitnessTypes { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin unique email
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // Admin -> FitnessTypes (1-M)
            modelBuilder.Entity<FitnessType>()
                .HasOne(f => f.Admin)
                .WithMany()
                .HasForeignKey(f => f.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin -> Plans (1-M)
            modelBuilder.Entity<Plan>()
                .HasOne(p => p.Admin)
                .WithMany()
                .HasForeignKey(p => p.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin -> Trainers (1-M)
            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.Admin)
                .WithMany()
                .HasForeignKey(t => t.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin -> Members (1-M)
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Admin)
                .WithMany()
                .HasForeignKey(m => m.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
            // Trainer <-> FitnessType (M-M)
            modelBuilder.Entity<Trainer>()
                .HasMany(t => t.FitnessTypes)
                .WithMany(f => f.Trainers)
                .UsingEntity<Dictionary<string, object>>(
                    "TrainerFitnessType",
                    j => j.HasOne<FitnessType>().WithMany().HasForeignKey("FitnessTypeId"),
                    j => j.HasOne<Trainer>().WithMany().HasForeignKey("TrainerId"),
                    je =>
                    {
                        je.HasKey("TrainerId", "FitnessTypeId");
                        je.HasData(
                            new { TrainerId = 1, FitnessTypeId = 1 },
                            new { TrainerId = 2, FitnessTypeId = 2 },
                            new { TrainerId = 3, FitnessTypeId = 3 },
                            new { TrainerId = 4, FitnessTypeId = 4 }
                        );
                    });

            // FitnessType -> Plans (1-M)
            modelBuilder.Entity<Plan>()
                .HasOne(p => p.FitnessType)
                .WithMany(f => f.Plans)
                .HasForeignKey(p => p.FitnessTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Plan -> Members (1-M)
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Plan)
                .WithMany(p => p.Members)
                .HasForeignKey(m => m.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Trainer -> Members (1-M)
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Trainer)
                .WithMany(t => t.Members)
                .HasForeignKey(m => m.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Data for Admin to prevent FK errors
            modelBuilder.Entity<Admin>().HasData(
                new Admin 
                { 
                    Id = 1, 
                    FullName = "Seeded Admin", 
                    Email = "seeded@gym.com", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                    Role = "Admin",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsActive = true
                }
            );

            // Seed Data for FitnessTypes
            modelBuilder.Entity<FitnessType>().HasData(
                new FitnessType { Id = 1, Name = "Cardio", Description = "Cardiovascular exercises, treadmill, cycling", ImageUrl = "/images/fitness/cardio.jpg", AdminId = 1 },
                new FitnessType { Id = 2, Name = "Weight Training", Description = "Muscle building and strength training", ImageUrl = "/images/fitness/weights.jpg", AdminId = 1 },
                new FitnessType { Id = 3, Name = "Yoga", Description = "Flexibility, balance and mental relaxation", ImageUrl = "/images/fitness/yoga.jpg", AdminId = 1 },
                new FitnessType { Id = 4, Name = "Pilates", Description = "Core strengthening and posture correction", ImageUrl = "/images/fitness/pilates.jpg", AdminId = 1 }
            );

            // Seed Data for Trainers
            modelBuilder.Entity<Trainer>().HasData(
                new Trainer { Id = 1, FullName = "John Smith", Phone = "0532 111 2233", Email = "john@gym.com", ExperienceYears = 5, AdminId = 1 },
                new Trainer { Id = 2, FullName = "Mike Miller", Phone = "0533 222 3344", Email = "mike@gym.com", ExperienceYears = 8, AdminId = 1 },
                new Trainer { Id = 3, FullName = "Sarah Connor", Phone = "0534 333 4455", Email = "sarah@gym.com", ExperienceYears = 6, AdminId = 1 },
                new Trainer { Id = 4, FullName = "Jane Doe", Phone = "0535 444 5566", Email = "jane@gym.com", ExperienceYears = 4, AdminId = 1 }
            );
 
            // Seed Data for Plans
            modelBuilder.Entity<Plan>().HasData(
                new Plan { Id = 1, Name = "Beginner Cardio", Price = 500, DurationMonths = 1, Description = "Cardio program for beginners", FitnessTypeId = 1, AdminId = 1 },
                new Plan { Id = 2, Name = "Pro Weights", Price = 800, DurationMonths = 3, Description = "Advanced weight training", FitnessTypeId = 2, AdminId = 1 },
                new Plan { Id = 3, Name = "Yoga Master", Price = 600, DurationMonths = 1, Description = "Comprehensive yoga training", FitnessTypeId = 3, AdminId = 1 },
                new Plan { Id = 4, Name = "Pilates Plus", Price = 700, DurationMonths = 2, Description = "Pilates for flexibility and strength", FitnessTypeId = 4, AdminId = 1 }
            );
 
            // Seed Data for Members
            modelBuilder.Entity<Member>().HasData(
                new Member { Id = 1, FullName = "Mark Johnson", Email = "mark@email.com", Phone = "0541 111 1111", RegistrationDate = new DateTime(2024, 1, 15), PlanId = 1, TrainerId = 1, AdminId = 1 },
                new Member { Id = 2, FullName = "Alice Brown", Email = "alice@email.com", Phone = "0542 222 2222", RegistrationDate = new DateTime(2024, 2, 20), PlanId = 2, TrainerId = 2, AdminId = 1 }
            );
        }
    }
}
