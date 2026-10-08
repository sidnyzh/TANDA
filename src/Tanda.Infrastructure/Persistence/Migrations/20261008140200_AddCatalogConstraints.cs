using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tanda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sizes_ProductId",
                table: "Sizes");

            migrationBuilder.DropIndex(
                name: "IX_Additions_ProductId",
                table: "Additions");

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_ProductId_Name",
                table: "Sizes",
                columns: new[] { "ProductId", "Name" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_DailyCapacities_CommittedNotGreaterThanTotal",
                table: "DailyCapacities",
                sql: "[CommittedEsfuerzo] <= [TotalEsfuerzo]");

            migrationBuilder.CreateIndex(
                name: "IX_Additions_ProductId_Name",
                table: "Additions",
                columns: new[] { "ProductId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sizes_ProductId_Name",
                table: "Sizes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DailyCapacities_CommittedNotGreaterThanTotal",
                table: "DailyCapacities");

            migrationBuilder.DropIndex(
                name: "IX_Additions_ProductId_Name",
                table: "Additions");

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_ProductId",
                table: "Sizes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Additions_ProductId",
                table: "Additions",
                column: "ProductId");
        }
    }
}
