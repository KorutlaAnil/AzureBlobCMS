using System;
using AzureBlobCMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AzureBlobCMS.Data
{
    public partial class WTTDbContext : DbContext
    {
        public WTTDbContext()
        {
        }

        public WTTDbContext(DbContextOptions<WTTDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Modules> Modules { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonLogin> PersonLogin { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=apphubittftest.database.windows.net;Database=wtt_new_dev;user id=apphubittftest;password=regit#11;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Modules>(entity =>
            {
                entity.HasKey(e => e.ModuleId);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);
            });

            modelBuilder.Entity<PersonLogin>(entity =>
            {
                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonLogin)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_Person_PersonLogIn");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
