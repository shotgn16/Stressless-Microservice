using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stressless_Service.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authorize",
                columns: table => new
                {
                    AuthorizeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    MACAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ClientID = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    LatestLogin = table.Column<string>(type: "TEXT", nullable: false),
                    Expires = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorize", x => x.AuthorizeID);
                });

            migrationBuilder.CreateTable(
                name: "CalenderEvents",
                columns: table => new
                {
                    EventID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Runtime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Event = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalenderEvents", x => x.EventID);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ConfigurationID = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    WorkingDays = table.Column<string>(type: "TEXT", nullable: false),
                    DayStartTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    DayEndTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    CalenderImport = table.Column<string>(type: "TEXT", nullable: false),
                    Calender = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationID);
                });

            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    PromptID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.PromptID);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    ReminderID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Time = table.Column<TimeOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.ReminderID);
                });

            migrationBuilder.CreateTable(
                name: "UsedPrompts",
                columns: table => new
                {
                    UsedPromptID = table.Column<Guid>(type: "TEXT", nullable: false),
                    PromptIdentification = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastUsed = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedPrompts", x => x.UsedPromptID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorize");

            migrationBuilder.DropTable(
                name: "CalenderEvents");

            migrationBuilder.DropTable(
                name: "CalenderModel");

            migrationBuilder.DropTable(
                name: "Prompts");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "UsedPrompts");

            migrationBuilder.DropTable(
                name: "Configuration");
        }
    }
}
