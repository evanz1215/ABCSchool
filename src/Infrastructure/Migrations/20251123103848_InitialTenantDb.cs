using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialTenantDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Multitenancy");

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "Multitenancy",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false), // FIX: 暫時在這直接修改,必須在EntityTypeConfiguration設定
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),    // FIX: 暫時在這直接修改,必須在EntityTypeConfiguration設定
                    ConnectionString = table.Column<string>(type: "nvarchar(500)", nullable: true), // FIX: 暫時在這直接修改,必須在EntityTypeConfiguration設定
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: false), // FIX: 暫時在這直接修改,必須在EntityTypeConfiguration設定
                    FirstName = table.Column<string>(type: "nvarchar(60)", nullable: false), // FIX: 暫時在這直接修改,必須在EntityTypeConfiguration設定
                    LastName = table.Column<string>(type: "nvarchar(60)", nullable: true), // FIX: 暫時在這直接修改,必須在EntityTypeConfiguration設定
                    ValidUpTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Identifier",
                schema: "Multitenancy",
                table: "Tenants",
                column: "Identifier",
                unique: true,
                filter: "[Identifier] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "Multitenancy");
        }
    }
}