using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LongPollingWorker.Migrations
{
    /// <inheritdoc />
    public partial class Add_File_DeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Files",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Files");
        }
    }
}
