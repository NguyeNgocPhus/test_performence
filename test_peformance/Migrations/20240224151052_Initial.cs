using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace test_peformance.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Salary = table.Column<decimal>(type: "numeric", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Department",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "phunn" });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "DepartmentId", "Name", "Salary" },
                values: new object[,]
                {
                    { 1, 1, "Employee_1", 10m },
                    { 2, 1, "Employee_2", 10m },
                    { 3, 1, "Employee_3", 10m },
                    { 4, 1, "Employee_4", 10m },
                    { 5, 1, "Employee_5", 10m },
                    { 6, 1, "Employee_6", 10m },
                    { 7, 1, "Employee_7", 10m },
                    { 8, 1, "Employee_8", 10m },
                    { 9, 1, "Employee_9", 10m },
                    { 10, 1, "Employee_10", 10m },
                    { 11, 1, "Employee_11", 10m },
                    { 12, 1, "Employee_12", 10m },
                    { 13, 1, "Employee_13", 10m },
                    { 14, 1, "Employee_14", 10m },
                    { 15, 1, "Employee_15", 10m },
                    { 16, 1, "Employee_16", 10m },
                    { 17, 1, "Employee_17", 10m },
                    { 18, 1, "Employee_18", 10m },
                    { 19, 1, "Employee_19", 10m },
                    { 20, 1, "Employee_20", 10m },
                    { 21, 1, "Employee_21", 10m },
                    { 22, 1, "Employee_22", 10m },
                    { 23, 1, "Employee_23", 10m },
                    { 24, 1, "Employee_24", 10m },
                    { 25, 1, "Employee_25", 10m },
                    { 26, 1, "Employee_26", 10m },
                    { 27, 1, "Employee_27", 10m },
                    { 28, 1, "Employee_28", 10m },
                    { 29, 1, "Employee_29", 10m },
                    { 30, 1, "Employee_30", 10m },
                    { 31, 1, "Employee_31", 10m },
                    { 32, 1, "Employee_32", 10m },
                    { 33, 1, "Employee_33", 10m },
                    { 34, 1, "Employee_34", 10m },
                    { 35, 1, "Employee_35", 10m },
                    { 36, 1, "Employee_36", 10m },
                    { 37, 1, "Employee_37", 10m },
                    { 38, 1, "Employee_38", 10m },
                    { 39, 1, "Employee_39", 10m },
                    { 40, 1, "Employee_40", 10m },
                    { 41, 1, "Employee_41", 10m },
                    { 42, 1, "Employee_42", 10m },
                    { 43, 1, "Employee_43", 10m },
                    { 44, 1, "Employee_44", 10m },
                    { 45, 1, "Employee_45", 10m },
                    { 46, 1, "Employee_46", 10m },
                    { 47, 1, "Employee_47", 10m },
                    { 48, 1, "Employee_48", 10m },
                    { 49, 1, "Employee_49", 10m },
                    { 50, 1, "Employee_50", 10m },
                    { 51, 1, "Employee_51", 10m },
                    { 52, 1, "Employee_52", 10m },
                    { 53, 1, "Employee_53", 10m },
                    { 54, 1, "Employee_54", 10m },
                    { 55, 1, "Employee_55", 10m },
                    { 56, 1, "Employee_56", 10m },
                    { 57, 1, "Employee_57", 10m },
                    { 58, 1, "Employee_58", 10m },
                    { 59, 1, "Employee_59", 10m },
                    { 60, 1, "Employee_60", 10m },
                    { 61, 1, "Employee_61", 10m },
                    { 62, 1, "Employee_62", 10m },
                    { 63, 1, "Employee_63", 10m },
                    { 64, 1, "Employee_64", 10m },
                    { 65, 1, "Employee_65", 10m },
                    { 66, 1, "Employee_66", 10m },
                    { 67, 1, "Employee_67", 10m },
                    { 68, 1, "Employee_68", 10m },
                    { 69, 1, "Employee_69", 10m },
                    { 70, 1, "Employee_70", 10m },
                    { 71, 1, "Employee_71", 10m },
                    { 72, 1, "Employee_72", 10m },
                    { 73, 1, "Employee_73", 10m },
                    { 74, 1, "Employee_74", 10m },
                    { 75, 1, "Employee_75", 10m },
                    { 76, 1, "Employee_76", 10m },
                    { 77, 1, "Employee_77", 10m },
                    { 78, 1, "Employee_78", 10m },
                    { 79, 1, "Employee_79", 10m },
                    { 80, 1, "Employee_80", 10m },
                    { 81, 1, "Employee_81", 10m },
                    { 82, 1, "Employee_82", 10m },
                    { 83, 1, "Employee_83", 10m },
                    { 84, 1, "Employee_84", 10m },
                    { 85, 1, "Employee_85", 10m },
                    { 86, 1, "Employee_86", 10m },
                    { 87, 1, "Employee_87", 10m },
                    { 88, 1, "Employee_88", 10m },
                    { 89, 1, "Employee_89", 10m },
                    { 90, 1, "Employee_90", 10m },
                    { 91, 1, "Employee_91", 10m },
                    { 92, 1, "Employee_92", 10m },
                    { 93, 1, "Employee_93", 10m },
                    { 94, 1, "Employee_94", 10m },
                    { 95, 1, "Employee_95", 10m },
                    { 96, 1, "Employee_96", 10m },
                    { 97, 1, "Employee_97", 10m },
                    { 98, 1, "Employee_98", 10m },
                    { 99, 1, "Employee_99", 10m },
                    { 100, 1, "Employee_100", 10m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
