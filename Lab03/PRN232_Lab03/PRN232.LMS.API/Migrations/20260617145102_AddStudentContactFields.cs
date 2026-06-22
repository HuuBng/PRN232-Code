using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN232.LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentContactFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "student_code",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "students");

            migrationBuilder.DropColumn(
                name: "student_code",
                table: "students");
        }
    }
}
