using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Reader.SQLite.ORM
{
    public partial class Entities : DbContext
    {
        public Entities()
        {
        }

        public Entities(DbContextOptions<Entities> options)
            : base(options)
        {
        }

        public virtual DbSet<Feed> Feeds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlite(@"data source=db");
                optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["sqlite"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feed>(entity =>
            {
                entity.ToTable("feed");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Url).HasColumnName("url");

                entity.Property(e => e.Xml).HasColumnName("xml");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
