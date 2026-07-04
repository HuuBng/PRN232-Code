using Microsoft.EntityFrameworkCore;
using PRN232.LMS.IdentityService.Entities;

namespace PRN232.LMS.IdentityService.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("users_pkey");
                entity.ToTable("users");
                entity.HasIndex(e => e.Username, "users_username_key").IsUnique();
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");
                entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash");
                entity.Property(e => e.Role).HasMaxLength(20).HasColumnName("role");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.RefreshTokenId).HasName("refresh_tokens_pkey");
                entity.ToTable("refresh_tokens");
                entity.HasIndex(e => e.Token, "refresh_tokens_token_key").IsUnique();
                entity.Property(e => e.RefreshTokenId).HasColumnName("refresh_token_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Token).HasMaxLength(255).HasColumnName("token");
                entity.Property(e => e.ExpiresAt).HasColumnType("timestamp without time zone").HasColumnName("expires_at");
                entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasColumnName("created_at");

                entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_refresh_tokens_user");
            });
        }
    }
}
