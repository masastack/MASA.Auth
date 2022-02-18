using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASA.Auth.UserDepartment.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "user");

            migrationBuilder.CreateTable(
                name: "departmentPositions",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Department = table.Column<Guid>(type: "TEXT", nullable: false),
                    PositionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departmentPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Describe = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationEventLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventTypeName = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    TimesSent = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platforms",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientSecret = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    PlatformType = table.Column<int>(type: "INTEGER", nullable: false),
                    VerifyType = table.Column<int>(type: "INTEGER", nullable: false),
                    IdentificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platformUsers",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Account = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    UserState = table.Column<int>(type: "INTEGER", nullable: false),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    PlatformId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platformUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "staffDepartments",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staffDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "staffPermission",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StaffId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PermissionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staffPermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "staffRoles",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StaffId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staffRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "staffs",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Account = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    UserState = table.Column<int>(type: "INTEGER", nullable: false),
                    FlowerName = table.Column<string>(type: "TEXT", nullable: true),
                    JobNumber = table.Column<string>(type: "TEXT", nullable: true),
                    StaffType = table.Column<int>(type: "INTEGER", nullable: false),
                    StaffState = table.Column<int>(type: "INTEGER", nullable: false),
                    OnboardingTime = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Salary = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teamRoles",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teamRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    Describe = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teamStaffs",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StaffId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teamStaffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChineseName = table.Column<string>(type: "TEXT", nullable: true),
                    EnglishName = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    IdentityNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Company = table.Column<string>(type: "TEXT", nullable: true),
                    PoliticalStatus = table.Column<string>(type: "TEXT", nullable: true),
                    IsMarried = table.Column<bool>(type: "INTEGER", nullable: true),
                    Sex = table.Column<int>(type: "INTEGER", nullable: false),
                    Age = table.Column<byte>(type: "INTEGER", nullable: true),
                    BirthDay = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Height = table.Column<short>(type: "INTEGER", nullable: true),
                    Weight = table.Column<short>(type: "INTEGER", nullable: true),
                    BloodType = table.Column<int>(type: "INTEGER", nullable: false),
                    MobilePhone = table.Column<string>(type: "TEXT", nullable: true),
                    OfficePhone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    WorkingAddress = table.Column<string>(type: "TEXT", nullable: true),
                    WorkingProvinceCode = table.Column<string>(type: "TEXT", nullable: true),
                    WorkingCityCode = table.Column<string>(type: "TEXT", nullable: true),
                    WorkingDistrictCode = table.Column<string>(type: "TEXT", nullable: true),
                    HouseholdRegisterAddress = table.Column<string>(type: "TEXT", nullable: true),
                    HouseholdRegisterProvinceCode = table.Column<string>(type: "TEXT", nullable: true),
                    HouseholdRegisterCityCode = table.Column<string>(type: "TEXT", nullable: true),
                    HouseholdRegisterDistrictCode = table.Column<string>(type: "TEXT", nullable: true),
                    ResidentialAddress = table.Column<string>(type: "TEXT", nullable: true),
                    ResidentialProvinceCode = table.Column<string>(type: "TEXT", nullable: true),
                    ResidentialCityCode = table.Column<string>(type: "TEXT", nullable: true),
                    ResidentialDistrictCode = table.Column<string>(type: "TEXT", nullable: true),
                    Photo = table.Column<string>(type: "TEXT", nullable: true),
                    IdentityFrontPhoto = table.Column<string>(type: "TEXT", nullable: true),
                    IdentityBackPhoto = table.Column<string>(type: "TEXT", nullable: true),
                    HouseholdRegisterPhoto = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Creator = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modifier = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "departmentPositions",
                schema: "user");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "user");

            migrationBuilder.DropTable(
                name: "IntegrationEventLog");

            migrationBuilder.DropTable(
                name: "platforms",
                schema: "user");

            migrationBuilder.DropTable(
                name: "platformUsers",
                schema: "user");

            migrationBuilder.DropTable(
                name: "positions",
                schema: "user");

            migrationBuilder.DropTable(
                name: "staffDepartments",
                schema: "user");

            migrationBuilder.DropTable(
                name: "staffPermission",
                schema: "user");

            migrationBuilder.DropTable(
                name: "staffRoles",
                schema: "user");

            migrationBuilder.DropTable(
                name: "staffs",
                schema: "user");

            migrationBuilder.DropTable(
                name: "teamRoles",
                schema: "user");

            migrationBuilder.DropTable(
                name: "teams",
                schema: "user");

            migrationBuilder.DropTable(
                name: "teamStaffs",
                schema: "user");

            migrationBuilder.DropTable(
                name: "users",
                schema: "user");
        }
    }
}
