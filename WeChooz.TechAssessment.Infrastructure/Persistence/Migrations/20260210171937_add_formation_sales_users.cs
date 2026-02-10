using Microsoft.EntityFrameworkCore.Migrations;
using WeChooz.TechAssessment.Domain.Users;

#nullable disable

namespace WeChooz.TechAssessment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_formation_sales_users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "LastName", "FirstName", "Email", "CompanyName", "Role" },
                values: new string[,]
                {
                    { 
                        Guid.NewGuid().ToString(), 
                        "F", 
                        "Form", 
                        "formation@test.com", 
                        "TestFormation", 
                        $"{(int)(PolicyRoles.Formation)}"
                    },
                    { 
                        Guid.NewGuid().ToString(), 
                        "S", 
                        "Sales", 
                        "sales@test.com", 
                        "TestSales", 
                        $"{(int)PolicyRoles.Sales}"  // Un seul rôle
                    }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Email",
                keyValues: new object[]
                {
                    "formation@test.com",
                    "sales@test.com"
                }
            );
        }
    }
}
