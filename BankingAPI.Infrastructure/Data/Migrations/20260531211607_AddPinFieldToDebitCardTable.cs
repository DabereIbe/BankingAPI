using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingAPI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPinFieldToDebitCardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pin",
                table: "DebitCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pin",
                table: "DebitCards");
        }
    }
}
