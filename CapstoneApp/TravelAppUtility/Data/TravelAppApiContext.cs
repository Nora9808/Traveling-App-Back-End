using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TravelAppUtility.Models;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Data
{
    public partial class TravelAppApiContext : DbContext
    {
        public TravelAppApiContext()
        {
        }

        public TravelAppApiContext(DbContextOptions<TravelAppApiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Collections> Collections { get; set; }
        public virtual DbSet<Continent> Continent { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Favorites> Favorites { get; set; }
        public virtual DbSet<Following> Following { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<UserEvents> UserEvents { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=localhost\\sqlexpress;Initial Catalog=TravelAppApi;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.CityId).ValueGeneratedNever();

                entity.Property(e => e.CityName).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.Continent)
                    .WithMany(p => p.City)
                    .HasForeignKey(d => d.ContinentId)
                    .HasConstraintName("FK__city__continent___3C69FB99");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.City)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__city__country_id__3D5E1FD2");
            });

            modelBuilder.Entity<Collections>(entity =>
            {
                entity.HasKey(e => e.CollectionId)
                    .HasName("PK__collecti__53D3A5CABA7EEE55");

                entity.Property(e => e.CollectionId).ValueGeneratedNever();

                entity.Property(e => e.Name).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__collectio__user___4222D4EF");
            });

            modelBuilder.Entity<Continent>(entity =>
            {
                entity.Property(e => e.ContinentId).ValueGeneratedNever();

                entity.Property(e => e.ContinentName).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId).ValueGeneratedNever();

                entity.Property(e => e.CountryName).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.Continent)
                    .WithMany(p => p.Country)
                    .HasForeignKey(d => d.ContinentId)
                    .HasConstraintName("FK__country__contine__398D8EEE");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.Property(e => e.EventsId).ValueGeneratedNever();

                entity.Property(e => e.Details).IsUnicode(false);

                entity.Property(e => e.Event).IsUnicode(false);

                entity.Property(e => e.Location).IsUnicode(false);
            });

            modelBuilder.Entity<Favorites>(entity =>
            {
                entity.Property(e => e.FavoritesId).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__favorites__user___44FF419A");
            });

            modelBuilder.Entity<Following>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Following)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__following__user___4F7CD00D");
            });

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.HasKey(e => e.RevewsId)
                    .HasName("PK__revews__3ACF4EC04D54EA8B");

                entity.Property(e => e.RevewsId).ValueGeneratedNever();

                entity.Property(e => e.Reviews1).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__revews__user_id__4CA06362");
            });

            modelBuilder.Entity<UserEvents>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserEvents)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__user_even__user___49C3F6B7");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__users__B9BE370F7C0FD765");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
