using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
namespace PRN232.LMS.Repositories.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<Enrollment> Enrollments { get; set; }

        public virtual DbSet<Semester> Semesters { get; set; }

        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<Subject> Subjects { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId).HasName("courses_pkey");

                entity.ToTable("courses");

                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CourseName)
                    .HasMaxLength(100)
                    .HasColumnName("course_name");
                entity.Property(e => e.SemesterId).HasColumnName("semester_id");
                entity.Property(e => e.SubjectId).HasColumnName("subject_id");

                entity.HasOne(d => d.Semester).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_courses_semester");

                entity.HasOne(d => d.Subject).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_courses_subject");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId).HasName("enrollments_pkey");

                entity.ToTable("enrollments");

                entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.EnrollDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("enroll_date");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");
                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("fk_enrollments_course");

                entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("fk_enrollments_student");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.SemesterId).HasName("semesters_pkey");

                entity.ToTable("semesters");

                entity.Property(e => e.SemesterId).HasColumnName("semester_id");
                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_date");
                entity.Property(e => e.SemesterName)
                    .HasMaxLength(100)
                    .HasColumnName("semester_name");
                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId).HasName("students_pkey");

                entity.ToTable("students");

                entity.HasIndex(e => e.Email, "students_email_key").IsUnique();

                entity.Property(e => e.StudentId).HasColumnName("student_id");
                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date_of_birth");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasColumnName("phone_number");
                entity.Property(e => e.StudentCode)
                    .HasMaxLength(20)
                    .HasColumnName("student_code");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.SubjectId).HasName("subjects_pkey");

                entity.ToTable("subjects");

                entity.HasIndex(e => e.SubjectCode, "subjects_subject_code_key").IsUnique();

                entity.Property(e => e.SubjectId).HasColumnName("subject_id");
                entity.Property(e => e.Credit).HasColumnName("credit");
                entity.Property(e => e.SubjectCode)
                    .HasMaxLength(20)
                    .HasColumnName("subject_code");
                entity.Property(e => e.SubjectName)
                    .HasMaxLength(100)
                    .HasColumnName("subject_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .HasColumnName("password_hash");
                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.RefreshTokenId).HasName("refresh_tokens_pkey");

                entity.ToTable("refresh_tokens");

                entity.HasIndex(e => e.Token, "refresh_tokens_token_key").IsUnique();

                entity.Property(e => e.RefreshTokenId).HasColumnName("refresh_token_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Token)
                    .HasMaxLength(255)
                    .HasColumnName("token");
                entity.Property(e => e.ExpiresAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("expires_at");
                entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");

                entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_refresh_tokens_user");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
