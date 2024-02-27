using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InterfaceServer.Repos;

public partial class FileTestContext : DbContext
{
    public FileTestContext()
    {
    }

    public FileTestContext(DbContextOptions<FileTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }

    public virtual DbSet<MusicFile> MusicFiles { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistHasMusicFile> PlaylistHasMusicFiles { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfmigrationsHistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__EFMigrationsHistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<MusicFile>(entity =>
        {
            entity.HasKey(e => e.IdMusicFile).HasName("PRIMARY");

            entity.ToTable("MusicFile");

            entity.Property(e => e.IdMusicFile).HasColumnName("idMusicFile");
            entity.Property(e => e.Albim).HasMaxLength(45);
            entity.Property(e => e.Artist).HasMaxLength(45);
            entity.Property(e => e.Title).HasMaxLength(45);
            entity.Property(e => e.Year).HasColumnType("year(4)");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.IdPlaylist).HasName("PRIMARY");

            entity.ToTable("Playlist");

            entity.HasIndex(e => e.UserIdUser, "fk_Playlist_User_idx");

            entity.Property(e => e.IdPlaylist).HasColumnName("idPlaylist");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.UserIdUser).HasColumnName("User_idUser");

            entity.HasOne(d => d.UserIdUserNavigation).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserIdUser)
                .HasConstraintName("playlist_ibfk_1");
        });

        modelBuilder.Entity<PlaylistHasMusicFile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Playlist_has_MusicFile");

            entity.HasIndex(e => e.MusicFileIdMusicFile, "fk_Playlist_has_MusicFile_MusicFile1_idx");

            entity.HasIndex(e => e.PlaylistIdPlaylist, "fk_Playlist_has_MusicFile_Playlist1_idx");

            entity.Property(e => e.MusicFileIdMusicFile).HasColumnName("MusicFile_idMusicFile");
            entity.Property(e => e.PlaylistIdPlaylist).HasColumnName("Playlist_idPlaylist");

            entity.HasOne(d => d.MusicFileIdMusicFileNavigation).WithMany()
                .HasForeignKey(d => d.MusicFileIdMusicFile)
                .HasConstraintName("playlist_has_musicfile_ibfk_1");

            entity.HasOne(d => d.PlaylistIdPlaylistNavigation).WithMany()
                .HasForeignKey(d => d.PlaylistIdPlaylist)
                .HasConstraintName("playlist_has_musicfile_ibfk_2");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("refreshToken");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.RefreshToken1)
                .HasMaxLength(1000)
                .HasColumnName("refreshToken");
            entity.Property(e => e.TokenId)
                .HasMaxLength(50)
                .HasColumnName("tokenId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.Property(e => e.IdRole)
                .HasColumnType("int(11)")
                .HasColumnName("idRole");
            entity.Property(e => e.Name).HasMaxLength(45);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PRIMARY");

            entity.ToTable("User");

            entity.HasIndex(e => e.RoleIdRole, "fk_User_Role1_idx");

            entity.Property(e => e.IdUser).HasColumnName("idUser");
            entity.Property(e => e.Login).HasMaxLength(45);
            entity.Property(e => e.Password).HasMaxLength(45);
            entity.Property(e => e.RoleIdRole)
                .HasColumnType("int(11)")
                .HasColumnName("Role_idRole");

            entity.HasOne(d => d.RoleIdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleIdRole)
                .HasConstraintName("user_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
