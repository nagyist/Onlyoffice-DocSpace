﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MSSql.FilesDbContextMSSql
{
    public partial class MSSqlFilesDbContext_Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "files_converts",
                columns: table => new
                {
                    input = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "LATIN1_GENERAL_100_CI_AS_SC_UTF8"),
                    output = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "LATIN1_GENERAL_100_CI_AS_SC_UTF8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_converts_pkey", x => new { x.input, x.output });
                });

            migrationBuilder.InsertData(
                table: "files_converts",
                columns: new[] { "input", "output" },
                values: new object[,]
                {
                    { ".csv", ".ods" },
                    { ".pps", ".odp" },
                    { ".pps", ".pdf" },
                    { ".pps", ".pptx" },
                    { ".ppsm", ".odp" },
                    { ".ppsm", ".pdf" },
                    { ".ppsm", ".pptx" },
                    { ".potx", ".pptx" },
                    { ".ppsx", ".odp" },
                    { ".ppsx", ".pptx" },
                    { ".ppt", ".odp" },
                    { ".ppt", ".pdf" },
                    { ".ppt", ".pptx" },
                    { ".pptm", ".odp" },
                    { ".pptm", ".pdf" },
                    { ".ppsx", ".pdf" },
                    { ".potx", ".pdf" },
                    { ".potx", ".odp" },
                    { ".potm", ".pptx" },
                    { ".ots", ".xlsx" },
                    { ".odt", ".docx" },
                    { ".odt", ".pdf" },
                    { ".odt", ".rtf" },
                    { ".odt", ".txt" },
                    { ".ott", ".docx" },
                    { ".ott", ".odt" },
                    { ".ott", ".pdf" },
                    { ".ott", ".rtf" },
                    { ".ott", ".txt" },
                    { ".pot", ".odp" },
                    { ".pot", ".pdf" },
                    { ".pot", ".pptx" },
                    { ".potm", ".odp" },
                    { ".potm", ".pdf" },
                    { ".pptm", ".pptx" },
                    { ".ots", ".pdf" },
                    { ".pptt", ".odp" },
                    { ".pptt", ".pptx" },
                    { ".xlst", ".xlsx" },
                    { ".xlst", ".csv" },
                    { ".xlst", ".ods" },
                    { ".xlt", ".csv" }
                });

            migrationBuilder.InsertData(
                table: "files_converts",
                columns: new[] { "input", "output" },
                values: new object[,]
                {
                    { ".xlt", ".ods" },
                    { ".xlt", ".pdf" },
                    { ".xlst", ".pdf" },
                    { ".xlt", ".xlsx" },
                    { ".xltm", ".ods" },
                    { ".xltm", ".pdf" },
                    { ".xltm", ".xlsx" },
                    { ".xltx", ".pdf" },
                    { ".xltx", ".csv" },
                    { ".xltx", ".ods" },
                    { ".xltm", ".csv" },
                    { ".xlsm", ".xlsx" },
                    { ".xlsm", ".ods" },
                    { ".xlsm", ".pdf" },
                    { ".pptx", ".odp" },
                    { ".pptx", ".pdf" },
                    { ".rtf", ".odp" },
                    { ".rtf", ".pdf" },
                    { ".rtf", ".docx" },
                    { ".rtf", ".txt" },
                    { ".txt", ".pdf" },
                    { ".txt", ".docx" },
                    { ".txt", ".odp" },
                    { ".txt", ".rtx" },
                    { ".xls", ".csv" },
                    { ".xls", ".ods" },
                    { ".xls", ".pdf" },
                    { ".xls", ".xlsx" },
                    { ".xlsm", ".csv" },
                    { ".pptt", ".pdf" },
                    { ".ots", ".ods" },
                    { ".ots", ".csv" },
                    { ".ods", ".xlsx" },
                    { ".dot", ".pdf" },
                    { ".dot", ".rtf" },
                    { ".dot", ".txt" },
                    { ".dotm", ".docx" },
                    { ".dotm", ".odt" },
                    { ".dotm", ".pdf" },
                    { ".dot", ".odt" },
                    { ".dotm", ".rtf" },
                    { ".dotx", ".docx" }
                });

            migrationBuilder.InsertData(
                table: "files_converts",
                columns: new[] { "input", "output" },
                values: new object[,]
                {
                    { ".dotx", ".odt" },
                    { ".dotx", ".pdf" },
                    { ".dotx", ".rtf" },
                    { ".dotx", ".txt" },
                    { ".epub", ".docx" },
                    { ".dotm", ".txt" },
                    { ".dot", ".docx" },
                    { ".docx", ".txt" },
                    { ".docx", ".rtf" },
                    { ".csv", ".pdf" },
                    { ".csv", ".xlsx" },
                    { ".doc", ".docx" },
                    { ".doc", ".odt" },
                    { ".doc", ".pdf" },
                    { ".doc", ".rtf" },
                    { ".doc", ".txt" },
                    { ".docm", ".docx" },
                    { ".docm", ".odt" },
                    { ".docm", ".pdf" },
                    { ".docm", ".rtf" },
                    { ".docm", ".txt" },
                    { ".doct", ".docx" },
                    { ".docx", ".odt" },
                    { ".docx", ".pdf" },
                    { ".epub", ".odt" },
                    { ".epub", ".pdf" },
                    { ".epub", ".rtf" },
                    { ".epub", ".txt" },
                    { ".html", ".pdf" },
                    { ".html", ".rtf" },
                    { ".html", ".txt" },
                    { ".mht", ".docx" },
                    { ".mht", ".odt" },
                    { ".mht", ".pdf" },
                    { ".mht", ".rtf" },
                    { ".mht", ".txt" },
                    { ".odp", ".pdf" },
                    { ".odp", ".pptx" },
                    { ".otp", ".odp" },
                    { ".otp", ".pdf" },
                    { ".otp", ".pptx" },
                    { ".ods", ".csv" }
                });

            migrationBuilder.InsertData(
                table: "files_converts",
                columns: new[] { "input", "output" },
                values: new object[,]
                {
                    { ".ods", ".pdf" },
                    { ".html", ".odt" },
                    { ".xltx", ".xlsx" },
                    { ".html", ".docx" },
                    { ".fodt", ".rtf" },
                    { ".fb2", ".docx" },
                    { ".fb2", ".odt" },
                    { ".fb2", ".pdf" },
                    { ".fb2", ".rtf" },
                    { ".fb2", ".txt" },
                    { ".fodp", ".odp" },
                    { ".fodp", ".pdf" },
                    { ".fodp", ".pptx" },
                    { ".fods", ".csv" },
                    { ".fods", ".ods" },
                    { ".fods", ".pdf" },
                    { ".fods", ".xlsx" },
                    { ".fodt", ".docx" },
                    { ".fodt", ".odt" },
                    { ".fodt", ".pdf" },
                    { ".fodt", ".txt" },
                    { ".xps", ".pdf" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "files_converts");
        }
    }
}
