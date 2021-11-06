using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace kroniiapi.Migrations
{
    public partial class ReInitialV4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationCategories",
                columns: table => new
                {
                    ApplicationCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CategoryName = table.Column<string>(type: "text", nullable: true),
                    SampleFileURL = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCategories", x => x.ApplicationCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "CostTypes",
                columns: table => new
                {
                    CostTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CostTypeName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostTypes", x => x.CostTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ModuleName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    NoOfSlot = table.Column<int>(type: "integer", nullable: false),
                    SlotDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IconURL = table.Column<string>(type: "text", nullable: true),
                    SyllabusURL = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MaxScore = table.Column<float>(type: "real", nullable: false),
                    PassingScore = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoleName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoomName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    AdministratorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Fullname = table.Column<string>(type: "text", nullable: true),
                    AvatarURL = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.AdministratorId);
                    table.ForeignKey(
                        name: "FK_Administrators_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Fullname = table.Column<string>(type: "text", nullable: true),
                    AvatarURL = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    DOB = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Facebook = table.Column<string>(type: "text", nullable: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                    table.ForeignKey(
                        name: "FK_Admins_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Fullname = table.Column<string>(type: "text", nullable: true),
                    AvatarURL = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Facebook = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Companies_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Trainers",
                columns: table => new
                {
                    TrainerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Fullname = table.Column<string>(type: "text", nullable: true),
                    AvatarURL = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    DOB = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.TrainerId);
                    table.ForeignKey(
                        name: "FK_Trainers_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClassName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartDay = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDay = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassId);
                    table.ForeignKey(
                        name: "FK_Classes_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Costs",
                columns: table => new
                {
                    CostId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "money", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CostTypeId = table.Column<int>(type: "integer", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costs", x => x.CostId);
                    table.ForeignKey(
                        name: "FK_Costs_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Costs_CostTypes_CostTypeId",
                        column: x => x.CostTypeId,
                        principalTable: "CostTypes",
                        principalColumn: "CostTypeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ExamName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExamDay = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DurationInMinute = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CancalledAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_Exams_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Exams_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CompanyRequests",
                columns: table => new
                {
                    CompanyRequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReportURL = table.Column<string>(type: "text", nullable: true),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRequests", x => x.CompanyRequestId);
                    table.ForeignKey(
                        name: "FK_CompanyRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    CalendarId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SyllabusSlot = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    ClassId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.CalendarId);
                    table.ForeignKey(
                        name: "FK_Calendars_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Calendars_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ClassModules",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WeightNumber = table.Column<float>(type: "real", nullable: false),
                    TrainerId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassModules", x => new { x.ClassId, x.ModuleId });
                    table.ForeignKey(
                        name: "FK_ClassModules_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassModules_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClassModules_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "TrainerId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DeleteClassRequests",
                columns: table => new
                {
                    DeleteClassRequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeleteClassRequests", x => x.DeleteClassRequestId);
                    table.ForeignKey(
                        name: "FK_DeleteClassRequests_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DeleteClassRequests_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Trainees",
                columns: table => new
                {
                    TraineeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Fullname = table.Column<string>(type: "text", nullable: true),
                    AvatarURL = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    DOB = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    Facebook = table.Column<string>(type: "text", nullable: true),
                    Wage = table.Column<decimal>(type: "money", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    OnBoard = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClassId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainees", x => x.TraineeId);
                    table.ForeignKey(
                        name: "FK_Trainees_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Trainees_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ApplicationURL = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Response = table.Column<string>(type: "text", nullable: true),
                    TraineeId = table.Column<int>(type: "integer", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: true),
                    ApplicationCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Applications_ApplicationCategories_ApplicationCategoryId",
                        column: x => x.ApplicationCategoryId,
                        principalTable: "ApplicationCategories",
                        principalColumn: "ApplicationCategoryId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Applications_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TraineeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_Attendances_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BonusAndPunishes",
                columns: table => new
                {
                    BonusAndPunishId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(type: "real", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TraineeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusAndPunishes", x => x.BonusAndPunishId);
                    table.ForeignKey(
                        name: "FK_BonusAndPunishes_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    TraineeId = table.Column<int>(type: "integer", nullable: false),
                    CertificateURL = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => new { x.ModuleId, x.TraineeId });
                    table.ForeignKey(
                        name: "FK_Certificates_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificates_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyRequestDetails",
                columns: table => new
                {
                    CompanyRequestId = table.Column<int>(type: "integer", nullable: false),
                    TraineeId = table.Column<int>(type: "integer", nullable: false),
                    Wage = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRequestDetails", x => new { x.CompanyRequestId, x.TraineeId });
                    table.ForeignKey(
                        name: "FK_CompanyRequestDetails_CompanyRequests_CompanyRequestId",
                        column: x => x.CompanyRequestId,
                        principalTable: "CompanyRequests",
                        principalColumn: "CompanyRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyRequestDetails_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TopicContent = table.Column<int>(type: "integer", nullable: false),
                    TopicObjective = table.Column<int>(type: "integer", nullable: false),
                    ApproriateTopicLevel = table.Column<int>(type: "integer", nullable: false),
                    TopicUsefulness = table.Column<int>(type: "integer", nullable: false),
                    TrainingMaterial = table.Column<int>(type: "integer", nullable: false),
                    TrainerKnowledge = table.Column<int>(type: "integer", nullable: false),
                    SubjectCoverage = table.Column<int>(type: "integer", nullable: false),
                    InstructionAndCommunicate = table.Column<int>(type: "integer", nullable: false),
                    TrainerSupport = table.Column<int>(type: "integer", nullable: false),
                    Logistics = table.Column<int>(type: "integer", nullable: false),
                    InformationToTrainees = table.Column<int>(type: "integer", nullable: false),
                    AdminSupport = table.Column<int>(type: "integer", nullable: false),
                    OtherComment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TraineeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Marks",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    TraineeId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => new { x.ModuleId, x.TraineeId });
                    table.ForeignKey(
                        name: "FK_Marks_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Marks_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeExams",
                columns: table => new
                {
                    TraineeId = table.Column<int>(type: "integer", nullable: false),
                    ExamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeExams", x => new { x.TraineeId, x.ExamId });
                    table.ForeignKey(
                        name: "FK_TraineeExams_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeExams_Trainees_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Trainees",
                        principalColumn: "TraineeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ApplicationCategories",
                columns: new[] { "ApplicationCategoryId", "CategoryName", "SampleFileURL" },
                values: new object[,]
                {
                    { 1, "Đơn đề nghị thôi học", "https://mega.nz/file/FZwxVYDY#osv1wM3x-JQ6jilW4zOO65eZA0_xFuiUqCey2G3Uprw" },
                    { 2, "Đơn chuyển cơ sở", "https://mega.nz/file/ZNpRTArA#y3JkAukljPtNsvINoub99zs6PEydE-OUIyqDyRLSx6I" },
                    { 3, "Đơn chuyển ngành học", "https://mega.nz/file/AIw1EIab#wjMq0nv4p2LyVebEbLcKXII4uXyF0xtaUXyBN2yUyU0" },
                    { 4, "Đơn bảo lưu học phần", "https://mega.nz/file/cVoBGC4R#1pgTUuPfGvk1abJZMb_MUsZ4d_3UgBqKOMNDVlm2Auo" },
                    { 5, "Đơn đăng ký thi cải thiện điểm", "https://mega.nz/file/lcxBGSLK#zl6kU7vF9dHvk203H1sv4gb-SjRe5EHyATFqRZF9XjI" },
                    { 6, "Đơn xác nhận thực tập", "https://mega.nz/file/FZoRjYiZ#kcbdQ0Mb4jzhNSXLP0jQGaJZgtSmJ3SIy0QD2ddpi4Q" },
                    { 7, "Đơn khiếu nại điểm danh", "https://mega.nz/file/tRphHCCJ#3kqNCGZT9XNzDNAe4WDYI2tOMqw_WI7sGR-cdGKHsz0" }
                });

            migrationBuilder.InsertData(
                table: "CostTypes",
                columns: new[] { "CostTypeId", "CostTypeName" },
                values: new object[,]
                {
                    { 1, "Cơ sở vật chất" },
                    { 2, "Tổ chức Event" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 5, "Company" },
                    { 4, "Trainee" },
                    { 3, "Trainer" },
                    { 2, "Admin" },
                    { 1, "Administrator" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "RoomName" },
                values: new object[,]
                {
                    { 1, "G101" },
                    { 2, "G102" },
                    { 3, "G103" },
                    { 4, "G104" },
                    { 5, "G105" },
                    { 6, "B101" },
                    { 7, "B102" },
                    { 8, "B103" },
                    { 9, "B104" },
                    { 10, "B105" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_RoleId",
                table: "Administrators",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_RoleId",
                table: "Admins",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AdminId",
                table: "Applications",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationCategoryId",
                table: "Applications",
                column: "ApplicationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_TraineeId",
                table: "Applications",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TraineeId",
                table: "Attendances",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusAndPunishes_TraineeId",
                table: "BonusAndPunishes",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_ClassId",
                table: "Calendars",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_ModuleId",
                table: "Calendars",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_TraineeId",
                table: "Certificates",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_AdminId",
                table: "Classes",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassModules_ModuleId",
                table: "ClassModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassModules_RoomId",
                table: "ClassModules",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassModules_TrainerId",
                table: "ClassModules",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_RoleId",
                table: "Companies",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRequestDetails_TraineeId",
                table: "CompanyRequestDetails",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRequests_CompanyId",
                table: "CompanyRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Costs_AdminId",
                table: "Costs",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Costs_CostTypeId",
                table: "Costs",
                column: "CostTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeleteClassRequests_AdminId",
                table: "DeleteClassRequests",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_DeleteClassRequests_ClassId",
                table: "DeleteClassRequests",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_AdminId",
                table: "Exams",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ModuleId",
                table: "Exams",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_TraineeId",
                table: "Feedbacks",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_TraineeId",
                table: "Marks",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeExams_ExamId",
                table: "TraineeExams",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_ClassId",
                table: "Trainees",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_RoleId",
                table: "Trainees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_RoleId",
                table: "Trainers",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "BonusAndPunishes");

            migrationBuilder.DropTable(
                name: "Calendars");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "ClassModules");

            migrationBuilder.DropTable(
                name: "CompanyRequestDetails");

            migrationBuilder.DropTable(
                name: "Costs");

            migrationBuilder.DropTable(
                name: "DeleteClassRequests");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropTable(
                name: "TraineeExams");

            migrationBuilder.DropTable(
                name: "ApplicationCategories");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Trainers");

            migrationBuilder.DropTable(
                name: "CompanyRequests");

            migrationBuilder.DropTable(
                name: "CostTypes");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Trainees");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
