using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.DataAccessLayer.Migrations
{
    public partial class AddUniqueRegistrationIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.DropIndex(
                name: "IX_ParticipantEvents_ParticipantEmailId",
                table: "ParticipantEvents");

            migrationBuilder.CreateIndex(
                name: "UX_ParticipantEvents_Email_Event",
                table: "ParticipantEvents",
                columns: new[] { "ParticipantEmailId", "EventId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_ParticipantEvents_Email_Event",
                table: "ParticipantEvents");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantEvents_ParticipantEmailId",
                table: "ParticipantEvents",
                column: "ParticipantEmailId");
        }
    }
}
