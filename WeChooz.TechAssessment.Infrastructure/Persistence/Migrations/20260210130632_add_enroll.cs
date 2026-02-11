using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeChooz.TechAssessment.Infrastructure.Persistence.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class add_enroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Sessions_SessionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Participants_SessionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "SessionEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionEnrollments_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionEnrollments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionEnrollments_EnrollmentDate",
                table: "SessionEnrollments",
                column: "EnrollmentDate");

            migrationBuilder.CreateIndex(
                name: "IX_SessionEnrollments_SessionId_UserId",
                table: "SessionEnrollments",
                columns: new[] { "SessionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionEnrollments_UserId",
                table: "SessionEnrollments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionEnrollments");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_SessionId",
                table: "Users",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Sessions_SessionId",
                table: "Users",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
