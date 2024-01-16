using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stressless_Service.Migrations
{
    /// <inheritdoc />
    public partial class CreateSQLITE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authorize",
                columns: table => new
                {
                    AuthorizeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    EventID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    ConfigurationID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    PromptID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    ReminderID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    UsedPromptID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PromptID = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUsed = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.PromptID);
                    table.ForeignKey(
                        name: "FK_UsedPromps",
                        column: x => x.PromptID,
                        principalTable: "UsedPrompts",
                        principalColumn: "PromptID");
                });

            migrationBuilder.CreateTable(
                name: "Calender",
                columns: table => new
                {
                    CalenderID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfigurationID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    EventDate = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationID);
                    table.ForeignKey(
                        name: "FK_Calender",
                        column: x => x.ConfigurationID,
                        principalTable: "Calender",
                        principalColumn: "ConfigurationID");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorize");

            migrationBuilder.DropTable(
                name: "Calender");

            migrationBuilder.DropTable(
                name: "CalenderEvents");

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
