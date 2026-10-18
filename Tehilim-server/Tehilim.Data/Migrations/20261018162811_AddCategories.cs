using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tehilim.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Chapters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1000', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Books",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookCategories",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "integer", nullable: false),
                    CategoriesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCategories", x => new { x.BooksId, x.CategoriesId });
                    table.ForeignKey(
                        name: "FK_BookCategories_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCategories_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "רפואה" },
                    { 2, "הצלחה" },
                    { 3, "עילוי נשמה" },
                    { 4, "ישועה" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "מנהל המערכת", "admin@tehilim.com", "AQAAAAIAAYagAAAAEGO1sYZjxGax9rbvDux+fNfEgQaftHPl3mDIiWz/UJlkI91IXm7jpRpWJOpljtNskQ==", "Admin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "משתמש לדוגמה", "user@tehilim.com", "AQAAAAIAAYagAAAAEGO1sYZjxGax9rbvDux+fNfEgQaftHPl3mDIiWz/UJlkI91IXm7jpRpWJOpljtNskQ==", "User" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "CompletedAt", "CreatedAt", "IsPublic", "OwnerId", "Purpose", "Status" },
                values: new object[] { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, "לרפואת כל חולי ישראל", "Open" });

            migrationBuilder.InsertData(
                table: "BookCategories",
                columns: new[] { "BooksId", "CategoriesId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Chapters",
                columns: new[] { "Id", "BookId", "Number", "Status", "TakenAt", "TakenByName", "TakenByUserId", "Version" },
                values: new object[,]
                {
                    { 1, 1, 1, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, 1, 2, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000002") },
                    { 3, 1, 3, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000003") },
                    { 4, 1, 4, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000004") },
                    { 5, 1, 5, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000005") },
                    { 6, 1, 6, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000006") },
                    { 7, 1, 7, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000007") },
                    { 8, 1, 8, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000008") },
                    { 9, 1, 9, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000009") },
                    { 10, 1, 10, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000010") },
                    { 11, 1, 11, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000011") },
                    { 12, 1, 12, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000012") },
                    { 13, 1, 13, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000013") },
                    { 14, 1, 14, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000014") },
                    { 15, 1, 15, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000015") },
                    { 16, 1, 16, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000016") },
                    { 17, 1, 17, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000017") },
                    { 18, 1, 18, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000018") },
                    { 19, 1, 19, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000019") },
                    { 20, 1, 20, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000020") },
                    { 21, 1, 21, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000021") },
                    { 22, 1, 22, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000022") },
                    { 23, 1, 23, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000023") },
                    { 24, 1, 24, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000024") },
                    { 25, 1, 25, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000025") },
                    { 26, 1, 26, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000026") },
                    { 27, 1, 27, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000027") },
                    { 28, 1, 28, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000028") },
                    { 29, 1, 29, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000029") },
                    { 30, 1, 30, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000030") },
                    { 31, 1, 31, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000031") },
                    { 32, 1, 32, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000032") },
                    { 33, 1, 33, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000033") },
                    { 34, 1, 34, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000034") },
                    { 35, 1, 35, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000035") },
                    { 36, 1, 36, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000036") },
                    { 37, 1, 37, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000037") },
                    { 38, 1, 38, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000038") },
                    { 39, 1, 39, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000039") },
                    { 40, 1, 40, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000040") },
                    { 41, 1, 41, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000041") },
                    { 42, 1, 42, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000042") },
                    { 43, 1, 43, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000043") },
                    { 44, 1, 44, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000044") },
                    { 45, 1, 45, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000045") },
                    { 46, 1, 46, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000046") },
                    { 47, 1, 47, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000047") },
                    { 48, 1, 48, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000048") },
                    { 49, 1, 49, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000049") },
                    { 50, 1, 50, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000050") },
                    { 51, 1, 51, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000051") },
                    { 52, 1, 52, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000052") },
                    { 53, 1, 53, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000053") },
                    { 54, 1, 54, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000054") },
                    { 55, 1, 55, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000055") },
                    { 56, 1, 56, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000056") },
                    { 57, 1, 57, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000057") },
                    { 58, 1, 58, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000058") },
                    { 59, 1, 59, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000059") },
                    { 60, 1, 60, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000060") },
                    { 61, 1, 61, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000061") },
                    { 62, 1, 62, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000062") },
                    { 63, 1, 63, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000063") },
                    { 64, 1, 64, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000064") },
                    { 65, 1, 65, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000065") },
                    { 66, 1, 66, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000066") },
                    { 67, 1, 67, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000067") },
                    { 68, 1, 68, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000068") },
                    { 69, 1, 69, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000069") },
                    { 70, 1, 70, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000070") },
                    { 71, 1, 71, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000071") },
                    { 72, 1, 72, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000072") },
                    { 73, 1, 73, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000073") },
                    { 74, 1, 74, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000074") },
                    { 75, 1, 75, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000075") },
                    { 76, 1, 76, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000076") },
                    { 77, 1, 77, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000077") },
                    { 78, 1, 78, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000078") },
                    { 79, 1, 79, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000079") },
                    { 80, 1, 80, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000080") },
                    { 81, 1, 81, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000081") },
                    { 82, 1, 82, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000082") },
                    { 83, 1, 83, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000083") },
                    { 84, 1, 84, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000084") },
                    { 85, 1, 85, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000085") },
                    { 86, 1, 86, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000086") },
                    { 87, 1, 87, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000087") },
                    { 88, 1, 88, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000088") },
                    { 89, 1, 89, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000089") },
                    { 90, 1, 90, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000090") },
                    { 91, 1, 91, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000091") },
                    { 92, 1, 92, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000092") },
                    { 93, 1, 93, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000093") },
                    { 94, 1, 94, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000094") },
                    { 95, 1, 95, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000095") },
                    { 96, 1, 96, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000096") },
                    { 97, 1, 97, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000097") },
                    { 98, 1, 98, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000098") },
                    { 99, 1, 99, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000099") },
                    { 100, 1, 100, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000100") },
                    { 101, 1, 101, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000101") },
                    { 102, 1, 102, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000102") },
                    { 103, 1, 103, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000103") },
                    { 104, 1, 104, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000104") },
                    { 105, 1, 105, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000105") },
                    { 106, 1, 106, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000106") },
                    { 107, 1, 107, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000107") },
                    { 108, 1, 108, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000108") },
                    { 109, 1, 109, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000109") },
                    { 110, 1, 110, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000110") },
                    { 111, 1, 111, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000111") },
                    { 112, 1, 112, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000112") },
                    { 113, 1, 113, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000113") },
                    { 114, 1, 114, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000114") },
                    { 115, 1, 115, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000115") },
                    { 116, 1, 116, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000116") },
                    { 117, 1, 117, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000117") },
                    { 118, 1, 118, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000118") },
                    { 119, 1, 119, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000119") },
                    { 120, 1, 120, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000120") },
                    { 121, 1, 121, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000121") },
                    { 122, 1, 122, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000122") },
                    { 123, 1, 123, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000123") },
                    { 124, 1, 124, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000124") },
                    { 125, 1, 125, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000125") },
                    { 126, 1, 126, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000126") },
                    { 127, 1, 127, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000127") },
                    { 128, 1, 128, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000128") },
                    { 129, 1, 129, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000129") },
                    { 130, 1, 130, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000130") },
                    { 131, 1, 131, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000131") },
                    { 132, 1, 132, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000132") },
                    { 133, 1, 133, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000133") },
                    { 134, 1, 134, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000134") },
                    { 135, 1, 135, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000135") },
                    { 136, 1, 136, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000136") },
                    { 137, 1, 137, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000137") },
                    { 138, 1, 138, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000138") },
                    { 139, 1, 139, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000139") },
                    { 140, 1, 140, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000140") },
                    { 141, 1, 141, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000141") },
                    { 142, 1, 142, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000142") },
                    { 143, 1, 143, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000143") },
                    { 144, 1, 144, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000144") },
                    { 145, 1, 145, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000145") },
                    { 146, 1, 146, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000146") },
                    { 147, 1, 147, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000147") },
                    { 148, 1, 148, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000148") },
                    { 149, 1, 149, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000149") },
                    { 150, 1, 150, "Available", null, null, null, new Guid("00000000-0000-0000-0000-000000000150") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCategories_CategoriesId",
                table: "BookCategories",
                column: "CategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "Chapters",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Chapters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1000', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Books",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
