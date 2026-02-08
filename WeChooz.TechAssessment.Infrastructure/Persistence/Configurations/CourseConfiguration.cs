using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeChooz.TechAssessment.Domain.Courses;

namespace WeChooz.TechAssessment.Infrastructure.Persistence.Configurations;

public class CourseConfiguration: IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ShortDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.LongDescription)
            .IsRequired();

        builder.Property(c => c.DurationInDays)
            .IsRequired();

        builder.Property(c => c.TargetAudience)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.MaxCapacity)
            .IsRequired();

        builder.Property(c => c.TrainerFirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.TrainerLastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(c => c.Sessions)
            .WithOne(s => s.Course)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}