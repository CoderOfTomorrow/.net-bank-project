using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Endava_Project.Server.Data.Migrations
{
    public partial class CashbyCodev1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashByCodeRepo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SourceWalletId = table.Column<Guid>(nullable: false),
                    SourceUserId = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExpireTime = table.Column<DateTime>(nullable: false),
                    GeneratedCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashByCodeRepo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashByCodeRepo");
        }
    }
}
