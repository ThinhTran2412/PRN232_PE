using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Q1.Models;

public partial class PePrn26spP8Context : DbContext
{
    public PePrn26spP8Context()
    {
    }

    public PePrn26spP8Context(DbContextOptions<PePrn26spP8Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Instructor> Instructors { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("PK__Instruct__94CEC126CFA16A98");

            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.InstructorName).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(100);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C12BF4BB24C984BB");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");

            entity.HasOne(d => d.Instructor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstructorId)
                .HasConstraintName("FK__Courses__InstructorID__2D27B809");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__A4AE64B8BDDE0123");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__C3905BAF987A80FC");

            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Semester).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Enrollme__Course__32E0915F");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Enrollme__Studen__31EC6D26");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
