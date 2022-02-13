using JWTTest.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace JWTTest.Migrations
{
    public partial class seedingroles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new string[]
                    {
                        Guid.NewGuid().ToString(),
                        nameof(Roles.User),
                        nameof(Roles.User).ToUpper(),
                        Guid.NewGuid().ToString()
                    }
                );
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new string[]
                    {
                        Guid.NewGuid().ToString(),
                        nameof(Roles.Admin),
                        nameof(Roles.Admin).ToUpper(),
                        Guid.NewGuid().ToString()
                    }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoles");
        }
    }
}
