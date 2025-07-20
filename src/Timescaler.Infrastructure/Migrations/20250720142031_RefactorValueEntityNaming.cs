using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timescaler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorValueEntityNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Values_ResultEntryId_Date_Desc",
                table: "Values",
                newName: "IX_Values_ResultId_Date_Desc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Values_ResultId_Date_Desc",
                table: "Values",
                newName: "IX_Values_ResultEntryId_Date_Desc");
        }
    }
}
