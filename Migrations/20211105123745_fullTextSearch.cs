using Microsoft.EntityFrameworkCore.Migrations;

namespace kroniiapi.Migrations
{
    public partial class fullTextSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamName",
                table: "Exams",
                column: "ExamName")
                .Annotation("Npgsql:TsVectorConfig", "simple");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Exams_ExamName",
                table: "Exams");
        }
    }
}
