using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudyProject.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Academic> Academics { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Engineer> Engineers { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<HeadOfDept> HeadOfDepts { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Student10> Student10s { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=study_practise;Username=postgres;Password=;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Academic>(entity =>
        {
            entity.HasKey(e => e.IdAcademics).HasName("academics_pkey");

            entity.ToTable("academics");

            entity.Property(e => e.IdAcademics).HasColumnName("id_academics");
            entity.Property(e => e.DateBirth).HasColumnName("date_birth");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.Specialization)
                .HasMaxLength(20)
                .HasColumnName("specialization");
            entity.Property(e => e.YearTitle).HasColumnName("year_title");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.IdApp).HasName("application_pkey");

            entity.ToTable("application");

            entity.Property(e => e.IdApp).HasColumnName("id_app");
            entity.Property(e => e.IdDisc).HasColumnName("id_disc");
            entity.Property(e => e.IdSpecialty).HasColumnName("id_specialty");

            entity.HasOne(d => d.IdDiscNavigation).WithMany(p => p.Applications)
                .HasForeignKey(d => d.IdDisc)
                .HasConstraintName("application_id_disc_fkey");

            entity.HasOne(d => d.IdSpecialtyNavigation).WithMany(p => p.Applications)
                .HasForeignKey(d => d.IdSpecialty)
                .HasConstraintName("application_id_specialty_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountries).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.Property(e => e.IdCountries).HasColumnName("id_countries");
            entity.Property(e => e.Capital)
                .HasMaxLength(20)
                .HasColumnName("capital");
            entity.Property(e => e.Continent)
                .HasMaxLength(20)
                .HasColumnName("continent");
            entity.Property(e => e.NameCountry)
                .HasMaxLength(20)
                .HasColumnName("name_country");
            entity.Property(e => e.Population).HasColumnName("population");
            entity.Property(e => e.Square).HasColumnName("square");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepart).HasName("department_pkey");

            entity.ToTable("department");

            entity.Property(e => e.IdDepart).HasColumnName("id_depart");
            entity.Property(e => e.Cipher)
                .HasMaxLength(3)
                .HasColumnName("cipher");
            entity.Property(e => e.IdFaculty).HasColumnName("id_faculty");
            entity.Property(e => e.NameDepart)
                .HasMaxLength(30)
                .HasColumnName("name_depart");

            entity.HasOne(d => d.IdFacultyNavigation).WithMany(p => p.Departments)
                .HasForeignKey(d => d.IdFaculty)
                .HasConstraintName("department_id_faculty_fkey");
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.IdDiscipline).HasName("discipline_pkey");

            entity.ToTable("discipline");

            entity.Property(e => e.IdDiscipline).HasColumnName("id_discipline");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Hours).HasColumnName("hours");
            entity.Property(e => e.IdDepart).HasColumnName("id_depart");
            entity.Property(e => e.NameDisc)
                .HasMaxLength(100)
                .HasColumnName("name_disc");

            entity.HasOne(d => d.IdDepartNavigation).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.IdDepart)
                .HasConstraintName("discipline_id_depart_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.TabNumEmployee).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.Property(e => e.TabNumEmployee)
                .ValueGeneratedNever()
                .HasColumnName("tab_num_employee");
            entity.Property(e => e.Chief).HasColumnName("chief");
            entity.Property(e => e.Fullname)
                .HasMaxLength(30)
                .HasColumnName("fullname");
            entity.Property(e => e.IdDepart).HasColumnName("id_depart");
            entity.Property(e => e.PositionEmp)
                .HasMaxLength(50)
                .HasColumnName("position_emp");
            entity.Property(e => e.Salary)
                .HasPrecision(10, 2)
                .HasColumnName("salary");

            entity.HasOne(d => d.ChiefNavigation).WithMany(p => p.InverseChiefNavigation)
                .HasForeignKey(d => d.Chief)
                .HasConstraintName("employee_chief_fkey");

            entity.HasOne(d => d.IdDepartNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.IdDepart)
                .HasConstraintName("employee_id_depart_fkey");
        });

        modelBuilder.Entity<Engineer>(entity =>
        {
            entity.HasKey(e => e.TabNumber).HasName("engineer_pkey");

            entity.ToTable("engineer");

            entity.Property(e => e.TabNumber)
                .ValueGeneratedNever()
                .HasColumnName("tab_number");
            entity.Property(e => e.Specialty)
                .HasMaxLength(50)
                .HasColumnName("specialty");

            entity.HasOne(d => d.TabNumberNavigation).WithOne(p => p.Engineer)
                .HasForeignKey<Engineer>(d => d.TabNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("engineer_tab_number_fkey");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.IdExam).HasName("exam_pkey");

            entity.ToTable("exam");

            entity.Property(e => e.IdExam).HasColumnName("id_exam");
            entity.Property(e => e.Classroom)
                .HasMaxLength(20)
                .HasColumnName("classroom");
            entity.Property(e => e.DisciplineCode).HasColumnName("discipline_code");
            entity.Property(e => e.ExamDate).HasColumnName("exam_date");
            entity.Property(e => e.ExaminerTab).HasColumnName("examiner_tab");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.StudentReg).HasColumnName("student_reg");

            entity.HasOne(d => d.DisciplineCodeNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.DisciplineCode)
                .HasConstraintName("exam_discipline_code_fkey");

            entity.HasOne(d => d.ExaminerTabNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.ExaminerTab)
                .HasConstraintName("exam_examiner_tab_fkey");

            entity.HasOne(d => d.StudentRegNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.StudentReg)
                .HasConstraintName("exam_student_reg_fkey");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.IdFaculty).HasName("faculty_pkey");

            entity.ToTable("faculty");

            entity.Property(e => e.IdFaculty).HasColumnName("id_faculty");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(5)
                .HasColumnName("abbreviation");
            entity.Property(e => e.NameFaculty)
                .HasMaxLength(30)
                .HasColumnName("name_faculty");
        });

        modelBuilder.Entity<HeadOfDept>(entity =>
        {
            entity.HasKey(e => e.TabNumber).HasName("head_of_dept_pkey");

            entity.ToTable("head_of_dept");

            entity.Property(e => e.TabNumber)
                .ValueGeneratedNever()
                .HasColumnName("tab_number");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");

            entity.HasOne(d => d.TabNumberNavigation).WithOne(p => p.HeadOfDept)
                .HasForeignKey<HeadOfDept>(d => d.TabNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("head_of_dept_tab_number_fkey");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("login_pkey");

            entity.ToTable("login");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Login1)
                .HasMaxLength(100)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Logins)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("login_id_user_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.IdSpecialty).HasName("specialty_pkey");

            entity.ToTable("specialty");

            entity.Property(e => e.IdSpecialty).HasColumnName("id_specialty");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.Direction)
                .HasMaxLength(100)
                .HasColumnName("direction");
            entity.Property(e => e.IdDepart).HasColumnName("id_depart");

            entity.HasOne(d => d.IdDepartNavigation).WithMany(p => p.Specialties)
                .HasForeignKey(d => d.IdDepart)
                .HasConstraintName("specialty_id_depart_fkey");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.RegNumber).HasName("student_pkey");

            entity.ToTable("student");

            entity.Property(e => e.RegNumber)
                .ValueGeneratedNever()
                .HasColumnName("reg_number");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.IdSpeciality).HasColumnName("id_speciality");

            entity.HasOne(d => d.IdSpecialityNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.IdSpeciality)
                .HasConstraintName("student_id_speciality_fkey");
        });

        modelBuilder.Entity<Student10>(entity =>
        {
            entity.HasKey(e => e.IdStudent).HasName("student10_pkey");

            entity.ToTable("student10");

            entity.Property(e => e.IdStudent).HasColumnName("id_student");
            entity.Property(e => e.School)
                .HasMaxLength(50)
                .HasColumnName("school");
            entity.Property(e => e.Scores).HasColumnName("scores");
            entity.Property(e => e.Subject)
                .HasMaxLength(50)
                .HasColumnName("subject");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TabNumber).HasName("teacher_pkey");

            entity.ToTable("teacher");

            entity.Property(e => e.TabNumber)
                .ValueGeneratedNever()
                .HasColumnName("tab_number");
            entity.Property(e => e.Degree)
                .HasMaxLength(50)
                .HasColumnName("degree");
            entity.Property(e => e.Rank)
                .HasMaxLength(50)
                .HasColumnName("rank");

            entity.HasOne(d => d.TabNumberNavigation).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.TabNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teacher_tab_number_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id_role_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
