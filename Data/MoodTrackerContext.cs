using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NewMoodTracker_AzureFunction.Models;

namespace NewMoodTracker_AzureFunction.Data;

public partial class MoodTrackerContext : DbContext
{
    public MoodTrackerContext()
    {
    }

    public MoodTrackerContext(DbContextOptions<MoodTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mood> Moods { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserMood> UserMoods { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:william.database.windows.net,1433;Initial Catalog=MoodTracker;Persist Security Info=False;User ID=williamTest;Password=\"Pslord$1A3\";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mood>(entity =>
        {
            entity.HasKey(e => e.MoodId).HasName("PK__Mood__E8B0F6F4F4BDBC3E");

            entity.ToTable("Mood");

            entity.Property(e => e.MoodId).HasColumnName("MoodID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CEAD86320");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534FA6DB29F").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK__UserLogi__4DDA281839DF26D6");

            entity.ToTable("UserLogin");

            entity.HasIndex(e => e.Username, "UQ__UserLogi__536C85E4C32051AB").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLogin__UserI__6A30C649");
        });

        modelBuilder.Entity<UserMood>(entity =>
        {
            entity.HasKey(e => e.MoodId).HasName("PK__UserMood__E8B0F6F4E9FF696C");

            entity.ToTable("UserMood");

            entity.Property(e => e.MoodId).HasColumnName("MoodID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MoodComments).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MoodTypeNavigation).WithMany(p => p.UserMoods)
                .HasForeignKey(d => d.MoodType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserMood__MoodTy__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
