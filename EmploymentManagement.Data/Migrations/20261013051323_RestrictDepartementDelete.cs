using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmploymentManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestrictDepartementDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes");

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes");

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
