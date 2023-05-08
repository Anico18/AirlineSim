using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SimAirline.Models
{
    public partial class DBEFContext : DbContext
    {
        public DBEFContext()
        {
        }

        public DBEFContext(DbContextOptions<DBEFContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airplane> Airplanes { get; set; } = null!;
        public virtual DbSet<BoardingPass> BoardingPasses { get; set; } = null!;
        public virtual DbSet<Flight> Flights { get; set; } = null!;
        public virtual DbSet<Passenger> Passengers { get; set; } = null!;
        public virtual DbSet<Purchase> Purchases { get; set; } = null!;
        public virtual DbSet<Seat> Seats { get; set; } = null!;
        public virtual DbSet<SeatType> SeatTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Airplane>(entity =>
            {
                entity.ToTable("airplane");

                entity.Property(e => e.AirplaneId)
                    .ValueGeneratedNever()
                    .HasColumnName("airplane_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<BoardingPass>(entity =>
            {
                entity.ToTable("boarding_pass");

                entity.HasIndex(e => e.FlightId, "flight_id");

                entity.HasIndex(e => e.PassengerId, "passenger_idx");

                entity.HasIndex(e => e.PurchaseId, "purchase");

                entity.HasIndex(e => e.SeatId, "seat");

                entity.HasIndex(e => e.SeatTypeId, "seattype");

                entity.Property(e => e.BoardingPassId)
                    .ValueGeneratedNever()
                    .HasColumnName("boarding_pass_id");

                entity.Property(e => e.FlightId).HasColumnName("flight_id");

                entity.Property(e => e.PassengerId).HasColumnName("passenger_id");

                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");

                entity.Property(e => e.SeatId).HasColumnName("seat_id");

                entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.BoardingPasses)
                    .HasForeignKey(d => d.FlightId)
                    .HasConstraintName("flight_id");

                entity.HasOne(d => d.Passenger)
                    .WithMany(p => p.BoardingPasses)
                    .HasForeignKey(d => d.PassengerId)
                    .HasConstraintName("passenger");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.BoardingPasses)
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("purchase");

                entity.HasOne(d => d.Seat)
                    .WithMany(p => p.BoardingPasses)
                    .HasForeignKey(d => d.SeatId)
                    .HasConstraintName("seat");

                entity.HasOne(d => d.SeatType)
                    .WithMany(p => p.BoardingPasses)
                    .HasForeignKey(d => d.SeatTypeId)
                    .HasConstraintName("seattype");
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.ToTable("flight");

                entity.Property(e => e.FlightId)
                    .ValueGeneratedNever()
                    .HasColumnName("flight_id");

                entity.Property(e => e.AirplaneId).HasColumnName("airplane_id");

                entity.Property(e => e.LandingAirport)
                    .HasMaxLength(255)
                    .HasColumnName("landing_airport");

                entity.Property(e => e.LandingDateTime).HasColumnName("landing_date_time");

                entity.Property(e => e.TakeoffAirport)
                    .HasMaxLength(255)
                    .HasColumnName("takeoff_airport");

                entity.Property(e => e.TakeoffDateTime).HasColumnName("takeoff_date_time");
            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.ToTable("passenger");

                entity.Property(e => e.PassengerId)
                    .ValueGeneratedNever()
                    .HasColumnName("passenger_id");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.Country)
                    .HasMaxLength(255)
                    .HasColumnName("country");

                entity.Property(e => e.Dni)
                    .HasMaxLength(255)
                    .HasColumnName("dni");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("purchase");

                entity.Property(e => e.PurchaseId)
                    .ValueGeneratedNever()
                    .HasColumnName("purchase_id");

                entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.ToTable("seat");

                entity.HasIndex(e => e.AirplaneId, "airplane");

                entity.Property(e => e.SeatId)
                    .ValueGeneratedNever()
                    .HasColumnName("seat_id");

                entity.Property(e => e.AirplaneId).HasColumnName("airplane_id");

                entity.Property(e => e.SeatColumn)
                    .HasMaxLength(2)
                    .HasColumnName("seat_column");

                entity.Property(e => e.SeatRow).HasColumnName("seat_row");

                entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");

                entity.HasOne(d => d.Airplane)
                    .WithMany(p => p.Seats)
                    .HasForeignKey(d => d.AirplaneId)
                    .HasConstraintName("airplane");
            });

            modelBuilder.Entity<SeatType>(entity =>
            {
                entity.ToTable("seat_type");

                entity.Property(e => e.SeatTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("seat_type_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
