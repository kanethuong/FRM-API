using Microsoft.EntityFrameworkCore.Migrations;

namespace kroniiapi.Migrations
{
    public partial class TrainerFacebook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "Trainers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "Trainers");
        }
    }
}
