using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LongPollingWorker.Migrations
{
    /// <inheritdoc />
    public partial class File_Document_And_Photo_Data_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "Files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OtherPhotoSizes",
                table: "Files",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "OtherPhotoSizes",
                table: "Files");

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "Files",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
