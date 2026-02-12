using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeChooz.TechAssessment.Domain.Enroll;

namespace WeChooz.TechAssessment.Infrastructure.Persistence.Configurations;

public class SessionEnrollConfiguration: IEntityTypeConfiguration<SessionEnroll>
{
    public void Configure(EntityTypeBuilder<SessionEnroll> builder)
    {
        builder.ToTable("SessionEnrollments");

        builder.HasKey(se => se.Id);

        builder.Property(se => se.EnrollmentDate)
            .IsRequired();
        
        builder.HasOne(se => se.Session)
            .WithMany(s => s.Enrollments )
            .HasForeignKey(se => se.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(se => se.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(se => se.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(se => new { se.SessionId, se.UserId }).IsUnique();
        builder.HasIndex(se => se.EnrollmentDate);
    }

}