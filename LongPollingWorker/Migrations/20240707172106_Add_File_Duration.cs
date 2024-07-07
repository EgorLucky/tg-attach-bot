using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LongPollingWorker.Migrations
{
    /// <inheritdoc />
    public partial class Add_File_Duration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Files",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Files");
        }
    }
}
