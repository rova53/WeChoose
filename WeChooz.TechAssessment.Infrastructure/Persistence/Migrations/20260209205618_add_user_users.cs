using Microsoft.EntityFrameworkCore.Migrations;
using BC = BCrypt.Net.BCrypt;
#nullable disable

namespace WeChooz.TechAssessment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_user_users : Migration
    {
        private string pass = BC.HashPassword("12345", BC.GenerateSalt(12));
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "SessionId", "LastName", "FirstName", "Email", "CompanyName", "Role", "Password" },
                values: new object[] 
                    { Guid.NewGuid(), null, "Rova", "A", "riri@email.com", "Ety",3,pass }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
            migrationBuilder.Sql("DELETE FROM Users WHERE Email = 'riri@email.com'");
        }
    }
}
