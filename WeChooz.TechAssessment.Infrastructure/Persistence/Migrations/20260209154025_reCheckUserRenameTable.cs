using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeChooz.TechAssessment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class reCheckUserRenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameTable(
                name: "Participants",
                newName: "Users"
            );
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Participants"
            );

        }
    }
}
