using Microsoft.EntityFrameworkCore.Migrations;

namespace Endava_Project.Server.Data.Migrations
{
    public partial class transactionnewupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationUserName",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUserName",
                table: "Transactions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationUserName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SourceUserName",
                table: "Transactions");
        }
    }
}
