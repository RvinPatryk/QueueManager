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
        }
    }
}