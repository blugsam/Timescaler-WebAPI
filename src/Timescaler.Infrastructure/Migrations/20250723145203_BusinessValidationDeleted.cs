using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timescaler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BusinessValidationDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Values_Date_Range",
                table: "Values");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Values_Date_Range",
                table: "Values",
                sql: "\"Date\" >= '2000-01-01T00:00:00Z'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Values_Date_Range",
                table: "Values");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Values_Date_Range",
                table: "Values",
                sql: "\"Date\" >= '2000-01-01T00:00:00Z' AND \"Date\" <= NOW()");
        }
    }
}
