namespace AutoService.Data.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles", table => table.HasComment("Роли пользователей"));
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", table => table.HasComment("Учётные записи сотрудников"));
        builder.Property(x => x.FullName).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.Property(x => x.Login).HasMaxLength(FieldLimits.Login).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(FieldLimits.PasswordHash).IsRequired();
        builder.HasIndex(x => x.Login).IsUnique();
        builder.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId);
    }
}

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments", table => table.HasComment("Отделы автосервиса"));
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

internal sealed class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("specializations", table => table.HasComment("Специализации механиков"));
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

internal sealed class MechanicConfiguration : IEntityTypeConfiguration<Mechanic>
{
    public void Configure(EntityTypeBuilder<Mechanic> builder)
    {
        builder.ToTable("mechanics", table => table.HasComment("Механики"));
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasOne(x => x.User).WithOne(x => x.Mechanic).HasForeignKey<Mechanic>(x => x.UserId);
        builder.HasOne(x => x.Department).WithMany(x => x.Mechanics).HasForeignKey(x => x.DepartmentId);
        builder.HasOne(x => x.Specialization).WithMany(x => x.Mechanics).HasForeignKey(x => x.SpecializationId);
    }
}
