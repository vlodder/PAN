using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PAN.context.Models;

public partial class GeipanContext : DbContext
{
    public GeipanContext()
    {
    }

    public GeipanContext(DbContextOptions<GeipanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Classement> Classement { get; set; }

    public virtual DbSet<Evenement> Evenement { get; set; }

    public virtual DbSet<Localisation> Localisation { get; set; }

    public virtual DbSet<Phenomene> Phenomene { get; set; }

    public virtual DbSet<Type> Type { get; set; }

    public virtual DbSet<Utilisateur> Utilisateur { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=2a03:5840:111:1024:1849:a413:e326:9df5;User ID=sa;Password=erty64%;Database=Geipan;TrustServerCertificate=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Classement>(entity =>
        {
            entity.HasKey(e => e.IdClassement).HasName("PK__Classeme__FFB96980C4B00A7E");

            entity.Property(e => e.IdClassement).ValueGeneratedNever();
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Evenement>(entity =>
        {
            entity.HasKey(e => e.IdEvenement).HasName("PK__Evenemen__300AD07E568856EA");

            entity.ToTable(tb => tb.HasTrigger("TRG_CheckObservationDate"));

            entity.Property(e => e.CompteRendu)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Descriptif)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(11, 8)");

            entity.HasOne(d => d.IdClassementNavigation).WithMany(p => p.Evenement)
                .HasForeignKey(d => d.IdClassement)
                .HasConstraintName("FK__Evenement__IdCla__36B12243");

            entity.HasOne(d => d.IdLocalisationNavigation).WithMany(p => p.Evenement)
                .HasForeignKey(d => d.IdLocalisation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Evenement__IdLoc__35BCFE0A");

            entity.HasOne(d => d.IdPhenomeneNavigation).WithMany(p => p.Evenement)
                .HasForeignKey(d => d.IdPhenomene)
                .HasConstraintName("FK__Evenement__IdPhe__37A5467C");

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.Evenement)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Evenement__IdTyp__38996AB5");

            entity.HasOne(d => d.IdUtilisateurNavigation).WithMany(p => p.Evenement)
                .HasForeignKey(d => d.IdUtilisateur)
                .HasConstraintName("FK__Evenement__IdUti__398D8EEE");
        });

        modelBuilder.Entity<Localisation>(entity =>
        {
            entity.HasKey(e => e.IdLocalisation).HasName("PK__Localisa__B42EF62EBB749F3A");

            entity.Property(e => e.IdLocalisation).ValueGeneratedNever();
            entity.Property(e => e.Ville)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Phenomene>(entity =>
        {
            entity.HasKey(e => e.IdPhenomene).HasName("PK__Phenomen__3E5AA130C0CD4EAA");

            entity.Property(e => e.IdPhenomene).ValueGeneratedNever();
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("PK__Type__9A39EABC3F0DB044");

            entity.Property(e => e.IdType).ValueGeneratedNever();
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.IdUtilisateur).HasName("PK__Utilisat__45A4C1579B5CE7D6");

            entity.Property(e => e.IdUtilisateur).ValueGeneratedNever();
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
