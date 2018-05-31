using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PX.WebWizard.Migrations
{
    public partial class AddLongRunInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LongRunInfos",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Abortable = table.Column<bool>(nullable: false),
                    Args = table.Column<string>(nullable: true),
                    Error = table.Column<string>(nullable: true),
                    JobId = table.Column<string>(nullable: true),
                    LongRunStatus = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LongRunInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LongRunInfos");
        }
    }
}
