using Microsoft.EntityFrameworkCore.Migrations;

namespace AiTools.DAL.Migrations
{
    public partial class AddInviteKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteKey",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteKey",
                table: "Users");
        }
    }
}
