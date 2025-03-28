using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UploadApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    DateCreate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetadatas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileMetadatas");
        }
    }
}
