using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LongPollingWorker.Migrations
{
    /// <inheritdoc />
    public partial class Update_File_KeyWords_Data_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "KeyWords",
                table: "Files",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "KeyWords",
                table: "Files",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);
        }
    }
}
