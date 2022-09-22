using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Learning.Data.Migrations
{
    public partial class AlterCourseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_AspNetUsers_TeacherId",
                table: "Course");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "Course",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_AspNetUsers_TeacherId",
                table: "Course",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_AspNetUsers_TeacherId",
                table: "Course");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "Course",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_AspNetUsers_TeacherId",
                table: "Course",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
