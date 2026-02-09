using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeChooz.TechAssessment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var courseId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Name", "ShortDescription", "LongDescription", "TargetAudience", "TrainerFirstName", "TrainerLastName", "DurationInDays", "MaxCapacity" },
                values: new object[] { courseId, "C# Avancé", "Cours avancé sur C#", "Ce cours couvre les concepts avancés de C#.", "CseElected", "Jean", "Dupont", 5, 20 }
            );
            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "CourseId", "DeliveryMode", "StartDate" },
                values: new object[] { sessionId, courseId, "InPerson", new DateTime(2026, 3, 1) }
            );
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "SessionId", "LastName", "FirstName", "Email", "CompanyName" },
                values: new object[] { Guid.NewGuid(), sessionId, "Martin", "A", "amartin@email.com", "TestCorp" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");
            migrationBuilder.Sql("DELETE FROM Users WHERE Email = 'amartin@email.com'");
            migrationBuilder.Sql("DELETE FROM Sessions WHERE DeliveryMode = 'InPerson' AND StartDate = '2026-03-01'");
            migrationBuilder.Sql("DELETE FROM Courses WHERE Name = 'C# Avancé'");
        }
    }
}
