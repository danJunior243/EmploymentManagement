using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmploymentManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UniquePaieParMois : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Paies_EmployeId",
                table: "Paies");

            migrationBuilder.CreateIndex(
                name: "IX_Paies_EmployeId_Mois",
                table: "Paies",
                columns: new[] { "EmployeId", "Mois" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Paies_EmployeId_Mois",
                table: "Paies");

            migrationBuilder.CreateIndex(
                name: "IX_Paies_EmployeId",
                table: "Paies",
                column: "EmployeId");
        }
    }
}
