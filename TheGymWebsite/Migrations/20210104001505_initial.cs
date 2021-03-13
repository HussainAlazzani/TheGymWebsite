using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TheGymWebsite.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    AddressLineOne = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLineTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FreePasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIssued = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreePasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gyms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GymName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLineOne = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLineTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gyms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MembershipDeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipDeals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DayName = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenHours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JobType = table.Column<int>(type: "int", nullable: false),
                    JobPeriod = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayInterval = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b5cb5390-960e-4976-858e-b594adf9dfdd", "4c632e6c-1676-4b48-80fe-5fce2ec60229", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AddressLineOne", "AddressLineTwo", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Postcode", "SecurityStamp", "Title", "Town", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "c162f710-1532-49e9-809d-5edec2bf1846", 0, "1 Admin Road", "Admin Area", "6fafc5ad-9d07-49e6-ae7b-e026bb97bafe", new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@admin.com", true, "AdminFirstName", 0, "AdminLastName", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEHNziAVqC15UEMOBTegRUzX+BtHmlZrPUbPRxegscNgAZ8nVdDebcw5WrkR81mascQ==", "00000000000", false, "AD1 2MN", "e1afb851-aa67-4798-b0db-5f737932f707", 0, "AdminTown", false, "admin@admin.com" },
                    { "0bc02f37-47aa-42b4-b823-62b225110f16", 0, "1 huss Road", "huss Area", "b3c516ee-f53c-4f15-8e92-745c016bec19", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "huss@yahoo.com", true, "hussFirstName", 0, "hussLastName", false, null, "HUSS@YAHOO.COM", "HUSS@YAHOO.COM", "AQAAAAEAACcQAAAAECAEwa+LzVJs5/Uk5ZXutjhiXparxpU0WqWwxzIyIaRuo4LU2hmNNNre4yTXl3wnuw==", "00000000000", false, "AD1 2MN", "5b1cd5ab-e30a-4988-9f37-1d00302ed3a6", 0, "hussTown", false, "huss@yahoo.com" },
                    { "75af9d64-1f80-4dbc-aed5-4e4b87e6b7cd", 0, "1 beky Road", "beky Area", "1fe8e533-0095-4945-aa7f-6f0f1b2f89ec", new DateTime(1950, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "beky@yahoo.com", true, "bekyFirstName", 1, "bekyLastName", false, null, "BEKY@YAHOO.COM", "BEKY@YAHOO.COM", "AQAAAAEAACcQAAAAEJYwTYxQrrY9sT4XybNEDzu6kBmLOz7a1gJ0dlRO1J56Rb7pcZP/8IKMu2jTVpJiqA==", "00000000000", false, "AD1 2MN", "ae085dfe-5755-451f-bf51-e1553863b49b", 0, "bekyTown", false, "beky@yahoo.com" },
                    { "0ac35167-b82f-4767-b798-99d1abfbf93e", 0, "1 alice Road", "alice Area", "2c8c6798-564f-42db-b653-61e50d4eed9f", new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "alice@yahoo.com", true, "aliceFirstName", 1, "aliceLastName", false, null, "ALICE@YAHOO.COM", "ALICE@YAHOO.COM", "AQAAAAEAACcQAAAAEHsLgo4JtuMy0KFv9JtxPMnIrPyqtxeAsTwD3z2oa5QXyY76QrZEPafqDSAiRN6FFQ==", "00000000000", false, "AD1 2MN", "1bd33475-0a4e-48c1-ae04-edea125e3599", 0, "aliceTown", false, "alice@yahoo.com" },
                    { "aa17ff45-53df-4c05-8bce-a1532c1c3265", 0, "1 seba Road", "seba Area", "0509295a-4f80-4d79-a3e0-c965dc5251d9", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "seba@yahoo.com", true, "sebaFirstName", 1, "sebaLastName", false, null, "SEBA@YAHOO.COM", "SEBA@YAHOO.COM", "AQAAAAEAACcQAAAAEMd++l/ZVoqL9W9j6hWJfKaMYLXqfpNy0CkWxx3waOh9Qt1fjtmGDyvhUiMcTX9EUQ==", "00000000000", false, "AD1 2MN", "220105c7-2e82-406d-b123-8bf1bda8c94a", 0, "sebaTown", false, "seba@yahoo.com" },
                    { "603c6c67-da52-49c4-89ca-a96d7d10eb6f", 0, "1 john Road", "john Area", "d9b3070b-b670-49fb-93ec-39508dbb6a94", new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john@yahoo.com", true, "johnFirstName", 0, "johnLastName", false, null, "JOHN@YAHOO.COM", "JOHN@YAHOO.COM", "AQAAAAEAACcQAAAAEESoebV+kC/St5EfutfmKrtsNUu2eWv58S6X/X0jVRm9xLnDH3pvOIg5y/qTa1jj7Q==", "00000000000", false, "AD1 2MN", "9b126d5f-1cc9-4c2b-8928-38ed34597ce8", 0, "johnTown", false, "john@yahoo.com" },
                    { "b575597d-8f76-41d5-99c6-4f88f4b2da6e", 0, "1 tom Road", "tom Area", "cea578a4-fc1a-4a99-8e73-18e37018190f", new DateTime(1993, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tom@yahoo.com", true, "tomFirstName", 0, "tomLastName", false, null, "TOM@YAHOO.COM", "TOM@YAHOO.COM", "AQAAAAEAACcQAAAAEDu8WkTd8wThM4+XckBdz+1YtrILaEyAIDnLRIzk6AwPzXTEb3nCY1cFTILS+jGACQ==", "00000000000", false, "AD1 2MN", "625029b1-5abb-4e96-a677-f7c6e6988f3c", 0, "tomTown", false, "tom@yahoo.com" },
                    { "523f8674-e924-446d-9db7-0966bdc6dea1", 0, "1 jack Road", "jack Area", "775c2503-c1ed-4439-9288-3d7ca9198ec7", new DateTime(1984, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jack@yahoo.com", true, "jackFirstName", 0, "jackLastName", false, null, "JACK@YAHOO.COM", "JACK@YAHOO.COM", "AQAAAAEAACcQAAAAEDmVfc+TeTo00TIn89OLjeuMbraw7HvCTLmgk5UwmJl85ETjXJE/dzFp15nBNHIrWw==", "00000000000", false, "AD1 2MN", "319a7104-6db1-490c-a422-b72a6dac2194", 0, "jackTown", false, "jack@yahoo.com" },
                    { "498c3c6b-5276-417b-a805-b9d41ec637e3", 0, "1 jam Road", "jam Area", "55f451ba-c00a-4872-bf64-cb59e964e007", new DateTime(1982, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jam@yahoo.com", true, "jamFirstName", 0, "jamLastName", false, null, "JAM@YAHOO.COM", "JAM@YAHOO.COM", "AQAAAAEAACcQAAAAEGVWtOx3RT+8h1N4IQSnp0h09cWgmqZ/E2yeMEpmJG5ni4Ps1efus4fngmpRqicf+Q==", "00000000000", false, "AD1 2MN", "69d9c1a0-a44a-4b75-bc8b-175046d43afb", 0, "jamTown", false, "jam@yahoo.com" },
                    { "346ab453-8f1a-4e6d-816b-4853ce09009f", 0, "1 mark Road", "mark Area", "de63d53b-fae9-4587-840d-10d1fc28761e", new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "mark@yahoo.com", true, "markFirstName", 0, "markLastName", false, null, "MARK@YAHOO.COM", "MARK@YAHOO.COM", "AQAAAAEAACcQAAAAEP3eFva6iCF4M00CnlRIhsZwWQiEySE77qzb97d7dL2lIDLJy1glyBmVJzVtATVxPQ==", "00000000000", false, "AD1 2MN", "d09a2d43-e876-4872-b1b9-1ad861154c68", 0, "markTown", false, "mark@yahoo.com" }
                });

            migrationBuilder.InsertData(
                table: "Gyms",
                columns: new[] { "Id", "AddressLineOne", "AddressLineTwo", "Email", "GymName", "Postcode", "Telephone", "Town" },
                values: new object[] { 1, "33 Oak road", "Erdon", "thegymbirmingham@yahoo.com", "The Gym", "B20 1EZ", "07739983984", "Birmingham" });

            migrationBuilder.InsertData(
                table: "MembershipDeals",
                columns: new[] { "Id", "Duration", "Price" },
                values: new object[,]
                {
                    { 1, 1, 10m },
                    { 2, 3, 20m },
                    { 3, 7, 100m },
                    { 4, 8, 160m }
                });

            migrationBuilder.InsertData(
                table: "OpenHours",
                columns: new[] { "Id", "CloseTime", "Date", "DayName", "Note", "OpenTime" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 22, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, new TimeSpan(0, 6, 0, 0, 0) },
                    { 2, new TimeSpan(0, 22, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, null, new TimeSpan(0, 6, 0, 0, 0) },
                    { 3, new TimeSpan(0, 22, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, null, new TimeSpan(0, 6, 0, 0, 0) },
                    { 4, new TimeSpan(0, 22, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, null, new TimeSpan(0, 6, 0, 0, 0) },
                    { 5, new TimeSpan(0, 22, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, null, new TimeSpan(0, 6, 0, 0, 0) },
                    { 6, new TimeSpan(0, 20, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, null, new TimeSpan(0, 8, 0, 0, 0) },
                    { 7, new TimeSpan(0, 20, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, new TimeSpan(0, 8, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "ManageBusiness", "True", "b5cb5390-960e-4976-858e-b594adf9dfdd" },
                    { 2, "ManageRoles", "True", "b5cb5390-960e-4976-858e-b594adf9dfdd" },
                    { 3, "ManageUsers", "True", "b5cb5390-960e-4976-858e-b594adf9dfdd" },
                    { 4, "IssueBans", "True", "b5cb5390-960e-4976-858e-b594adf9dfdd" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 10, "DateOfBirth", "01/01/1984", "523f8674-e924-446d-9db7-0966bdc6dea1" },
                    { 9, "DateOfBirth", "01/01/1993", "b575597d-8f76-41d5-99c6-4f88f4b2da6e" },
                    { 8, "DateOfBirth", "01/01/1994", "603c6c67-da52-49c4-89ca-a96d7d10eb6f" },
                    { 7, "DateOfBirth", "01/01/1970", "aa17ff45-53df-4c05-8bce-a1532c1c3265" },
                    { 6, "DateOfBirth", "01/01/1960", "0ac35167-b82f-4767-b798-99d1abfbf93e" },
                    { 4, "DateOfBirth", "01/01/2000", "0bc02f37-47aa-42b4-b823-62b225110f16" },
                    { 11, "DateOfBirth", "01/01/1982", "498c3c6b-5276-417b-a805-b9d41ec637e3" },
                    { 3, "MembershipExpiry", "31/12/9999", "c162f710-1532-49e9-809d-5edec2bf1846" },
                    { 2, "Employee", "04/01/2021", "c162f710-1532-49e9-809d-5edec2bf1846" },
                    { 1, "DateOfBirth", "01/01/2000", "c162f710-1532-49e9-809d-5edec2bf1846" },
                    { 5, "DateOfBirth", "01/01/1950", "75af9d64-1f80-4dbc-aed5-4e4b87e6b7cd" },
                    { 12, "DateOfBirth", "01/01/2010", "346ab453-8f1a-4e6d-816b-4853ce09009f" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "b5cb5390-960e-4976-858e-b594adf9dfdd", "c162f710-1532-49e9-809d-5edec2bf1846" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_UserId",
                table: "AttendanceRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipDeals_Duration",
                table: "MembershipDeals",
                column: "Duration",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AttendanceRecord");

            migrationBuilder.DropTable(
                name: "FreePasses");

            migrationBuilder.DropTable(
                name: "Gyms");

            migrationBuilder.DropTable(
                name: "MembershipDeals");

            migrationBuilder.DropTable(
                name: "OpenHours");

            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
