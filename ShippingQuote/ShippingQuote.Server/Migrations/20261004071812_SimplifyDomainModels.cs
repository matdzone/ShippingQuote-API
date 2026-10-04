using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingQuote.Server.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyDomainModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationCountryCode",
                table: "PricingRules");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "PricingRules");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Carriers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationCountryCode",
                table: "PricingRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "PricingRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Carriers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
