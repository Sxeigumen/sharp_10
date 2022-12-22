using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace lab10New;

public partial class DataBase : DbContext
{
    public DataBase()
    {
    }

    public DataBase(DbContextOptions<DataBase> options)
        : base(options)
    {
    }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Ticker> Tickers { get; set; }

    public virtual DbSet<TodayCondition> TodayConditions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=laba;User ID=sa;Password=HelloWorld10;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Price>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Ticker).WithMany(p => p.Prices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prices_Tickers");
        });

        modelBuilder.Entity<Ticker>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Ticker1).IsFixedLength();
        });

        modelBuilder.Entity<TodayCondition>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.State).IsFixedLength();

            entity.HasOne(d => d.Ticker).WithMany(p => p.TodayConditions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TodayCondition_Tickers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}