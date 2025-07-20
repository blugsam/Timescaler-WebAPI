using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timescaler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    FirstOperationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeDelta = table.Column<TimeSpan>(type: "interval", nullable: false),
                    AverageExecutionTime = table.Column<double>(type: "double precision", nullable: false),
                    AverageValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    MedianValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    MaxValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    MinValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.CheckConstraint("CK_Results_TimeDelta_Positive", "\"TimeDelta\" >= INTERVAL '0 seconds'");
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionTime = table.Column<double>(type: "double precision", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    ResultId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                    table.CheckConstraint("CK_Values_Date_Range", "\"Date\" >= '2000-01-01T00:00:00Z' AND \"Date\" <= NOW()");
                    table.CheckConstraint("CK_Values_ExecutionTime_Positive", "\"ExecutionTime\" >= 0");
                    table.CheckConstraint("CK_Values_Value_Positive", "\"Value\" >= 0");
                    table.ForeignKey(
                        name: "FK_Values_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_AverageExecutionTime",
                table: "Results",
                column: "AverageExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_Results_AverageValue",
                table: "Results",
                column: "AverageValue");

            migrationBuilder.CreateIndex(
                name: "IX_Results_FileName_Unique",
                table: "Results",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_FirstOperationDate",
                table: "Results",
                column: "FirstOperationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Values_ResultEntryId_Date_Desc",
                table: "Values",
                columns: new[] { "ResultId", "Date" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
