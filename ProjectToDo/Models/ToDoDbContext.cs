using Microsoft.EntityFrameworkCore;
using System;

namespace ProjectToDo.Models
{
    public class ToDoDbContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Konfigurasi tambahan jika diperlukan

            modelBuilder.Entity<User>()
                        .HasOne(u => u.Role)  
                        .WithMany(r => r.Users)  
                        .HasForeignKey(u => u.RoleId)  
                        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Project>()
                        .HasOne(p => p.User)
                        .WithMany(u => u.Projects)
                        .HasForeignKey(p => p.UserPMId)
                        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Task>()
                        .HasOne(u => u.User)
                        .WithMany(t => t.Tasks)
                        .HasForeignKey(u => u.UserId)
                        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Task>()
                        .HasOne(p => p.Project)
                        .WithMany(t => t.Tasks)
                        .HasForeignKey(p => p.ProjectId)
                        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Task>()
                        .HasOne(s => s.Status)
                        .WithMany(t => t.Tasks)
                        .HasForeignKey(s => s.StatusId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Developer" },
                new Role { Id = 2, Name = "QA" },
                new Role { Id = 3, Name = "PM" },
                new Role { Id = 4, Name = "Admin" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = "New" },
                new Status { Id = 2, Name = "On Progress" },
                new Status { Id = 3, Name = "In Testing" },
                new Status { Id = 4, Name = "Done" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Admin", Username = "admin", RoleId = 4, Password = "AQAAAAEAACcQAAAAEHR3oI+PVbeBtRJRHk8aCBBPU/wolKQLhDxmpW+1h64ytrF02fHY7UGEiZHpEZSZiw==" }
            );
        }
    }
}
