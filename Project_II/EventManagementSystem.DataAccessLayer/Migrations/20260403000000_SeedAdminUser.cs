using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.DataAccessLayer.Migrations
{
    
    public partial class SeedAdminUser : Migration
    {
        // Canonical BCrypt hash for "Admin@123" 
        private const string AdminPasswordHash =
            "$2a$11$IAF00m.BFFGKVKi.eFn3YO43CJ7box7Gm2S8LOO6UWRVlpzKLUWa2";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "EmailId", "Password", "Role", "UserName" },
                values: new object[] { "admin@ems.com", AdminPasswordHash, "Admin", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "EmailId",
                keyValue: "admin@ems.com");
        }
    }
}
