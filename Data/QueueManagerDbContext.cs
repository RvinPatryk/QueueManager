using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueManager.Models;
using System.IO;

namespace QueueManager.Data
{
    public class QueueManagerDbContext : DbContext
    {
        public DbSet<QueueTask> Tasks { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<QueueSettings> QueueSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = Path.Combine(
                AppContext.BaseDirectory,
                "QueueManager.db");

            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QueueTask>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Nazwa)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.Opis)
                    .HasMaxLength(2000);

                entity.Property(t => t.Autor)
                    .HasMaxLength(100);

                entity.Property(t => t.OsobaPrzypisana)
                    .HasMaxLength(100);

                entity.Property(t => t.Priorytet)
                    .IsRequired();

                entity.Property(t => t.Status)
                    .IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(user => user.Id);

                entity.Property(user => user.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(user => user.Username)
                    .IsUnique();

                entity.Property(user => user.PasswordHash)
                    .IsRequired();

                entity.Property(user => user.Role)
                    .IsRequired();
            });

            modelBuilder.Entity<QueueSettings>(entity =>
            {
                entity.HasKey(settings => settings.Id);

                entity.Property(settings => settings.SelectedAlgorithm)
                    .IsRequired();
            });

        }


    }
}