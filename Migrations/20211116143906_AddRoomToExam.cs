using Microsoft.EntityFrameworkCore.Migrations;

namespace kroniiapi.Migrations
{
    public partial class AddRoomToExam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Exams",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_RoomId",
                table: "Exams",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Rooms_RoomId",
                table: "Exams",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Rooms_RoomId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_RoomId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Exams");
        }
    }
}
