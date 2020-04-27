using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RomDatabase5
{
    public partial class RomDBContext : DbContext
    {
        public RomDBContext()
        {
        }

        public RomDBContext(DbContextOptions<RomDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Consoles> Consoles { get; set; }
        public virtual DbSet<Datfiles> Datfiles { get; set; }
        public virtual DbSet<Discs> Discs { get; set; }
        public virtual DbSet<Games> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=RomDB.sqlite");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Consoles>(entity =>
            {
                entity.ToTable("consoles");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<Datfiles>(entity =>
            {
                entity.ToTable("datfiles");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<Discs>(entity =>
            {
                entity.ToTable("discs");

                entity.HasIndex(x => x.Console)
                    .HasName("idx_discconsole");

                entity.HasIndex(x => x.Name)
                    .HasName("idx_discname");

                entity.HasIndex(x => new { x.Size, x.Crc, x.Sha1, x.Md5 })
                    .HasName("idx_discidentity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Console)
                    .HasColumnName("console")
                    .HasColumnType("INT");

                entity.Property(e => e.Crc).HasColumnName("crc");

                entity.Property(e => e.DatFile)
                    .HasColumnName("datFile")
                    .HasColumnType("INT");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Md5).HasColumnName("md5");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Sha1).HasColumnName("sha1");

                entity.Property(e => e.Size)
                    .HasColumnName("size")
                    .HasColumnType("INT");
            });

            modelBuilder.Entity<Games>(entity =>
            {
                entity.ToTable("games");

                entity.HasIndex(x => x.Console)
                    .HasName("idx_gameconsole");

                entity.HasIndex(x => x.Name)
                    .HasName("idx_gamename");

                entity.HasIndex(x => new { x.Size, x.Crc, x.Sha1, x.Md5 })
                    .HasName("idx_gameidentity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Console)
                    .HasColumnName("console")
                    .HasColumnType("INT");

                entity.Property(e => e.Crc).HasColumnName("crc");

                entity.Property(e => e.DatFile)
                    .HasColumnName("datFile")
                    .HasColumnType("INT");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Md5).HasColumnName("md5");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Sha1).HasColumnName("sha1");

                entity.Property(e => e.Size)
                    .HasColumnName("size")
                    .HasColumnType("INT");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
