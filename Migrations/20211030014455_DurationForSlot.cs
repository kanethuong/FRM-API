using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace kroniiapi.Migrations
{
    public partial class DurationForSlot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "SlotDuration",
                table: "Modules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlotDuration",
                table: "Modules");
        }
    }
}
