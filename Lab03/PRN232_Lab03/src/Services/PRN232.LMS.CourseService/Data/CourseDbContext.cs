using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Entities;

namespace PRN232.LMS.CourseService.Data
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId).HasName("courses_pkey");
                entity.ToTable("courses");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CourseName).HasMaxLength(100).HasColumnName("course_name");
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
                entity.Property(e => e.EnrollDate).HasColumnType("timestamp without time zone").HasColumnName("enroll_date");
                entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("fk_enrollments_course");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.SemesterId).HasName("semesters_pkey");
                entity.ToTable("semesters");
                entity.Property(e => e.SemesterId).HasColumnName("semester_id");
                entity.Property(e => e.SemesterName).HasMaxLength(100).HasColumnName("semester_name");
                entity.Property(e => e.StartDate).HasColumnType("timestamp without time zone").HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnType("timestamp without time zone").HasColumnName("end_date");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.SubjectId).HasName("subjects_pkey");
                entity.ToTable("subjects");
                entity.HasIndex(e => e.SubjectCode, "subjects_subject_code_key").IsUnique();
                entity.Property(e => e.SubjectId).HasColumnName("subject_id");
                entity.Property(e => e.SubjectCode).HasMaxLength(20).HasColumnName("subject_code");
                entity.Property(e => e.SubjectName).HasMaxLength(100).HasColumnName("subject_name");
                entity.Property(e => e.Credit).HasColumnName("credit");
            });
        }
    }
}
