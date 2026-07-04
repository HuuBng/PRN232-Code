using Microsoft.EntityFrameworkCore;
using PRN232.LMS.StudentService.Entities;

namespace PRN232.LMS.StudentService.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId).HasName("students_pkey");
                entity.ToTable("students");
                entity.HasIndex(e => e.Email, "students_email_key").IsUnique();
                entity.Property(e => e.StudentId).HasColumnName("student_id");
                entity.Property(e => e.FullName).HasMaxLength(100).HasColumnName("full_name");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
                entity.Property(e => e.DateOfBirth).HasColumnType("timestamp without time zone").HasColumnName("date_of_birth");
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).HasColumnName("phone_number");
                entity.Property(e => e.StudentCode).HasMaxLength(20).HasColumnName("student_code");
            });
        }
    }
}
