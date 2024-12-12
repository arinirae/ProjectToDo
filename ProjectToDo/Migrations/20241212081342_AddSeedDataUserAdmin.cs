using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectToDo.Migrations
{
    public partial class AddSeedDataUserAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Password", "RoleId", "Username" },
                values: new object[] { 1, "Admin", "AQAAAAEAACcQAAAAEHR3oI+PVbeBtRJRHk8aCBBPU/wolKQLhDxmpW+1h64ytrF02fHY7UGEiZHpEZSZiw==", 4, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
