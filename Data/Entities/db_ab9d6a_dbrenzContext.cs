using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EventMonitoring.ph.Data.Entities;

public partial class db_ab9d6a_dbrenzContext : DbContext
{
    public db_ab9d6a_dbrenzContext()
    {
    }

    public db_ab9d6a_dbrenzContext(DbContextOptions<db_ab9d6a_dbrenzContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EventAudience> EventAudiences { get; set; }

    public virtual DbSet<EventLine> EventLines { get; set; }

    public virtual DbSet<EventTitle> EventTitles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=SQL1002.site4now.net;Database=db_ab9d6a_dbrenz;User Id=db_ab9d6a_dbrenz_admin;Password=renzbayadog12345;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventAudience>(entity =>
        {
            entity.ToTable("EventAudience");

            entity.Property(e => e.EventRemarks)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.QrCode)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.EventTitle).WithMany(p => p.EventAudiences)
                .HasForeignKey(d => d.EventTitleId)
                .HasConstraintName("FK_EventAudience_EventTitle");
        });

        modelBuilder.Entity<EventLine>(entity =>
        {
            entity.ToTable("EventLine");

            entity.Property(e => e.EventLineName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EventTitle>(entity =>
        {
            entity.ToTable("EventTitle");

            entity.Property(e => e.EventTitleDescription)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EventTitleEndTimeDate).HasColumnType("datetime");
            entity.Property(e => e.EventTitleStartTimeDate).HasColumnType("datetime");
            entity.Property(e => e.EventTitleVenueName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.EventLine).WithMany(p => p.EventTitles)
                .HasForeignKey(d => d.EventLineId)
                .HasConstraintName("FK_EventTitle_EventLine");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
