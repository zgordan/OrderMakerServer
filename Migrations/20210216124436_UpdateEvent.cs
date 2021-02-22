using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "responsibility",
                table: "mtd_policy_forms",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.CreateTable(
                name: "mtd_event_subscribe",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    event_create = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    event_edit = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_event_subscribe", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_event_mtd_form",
                        column: x => x.mtd_form_id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "fk_mtd_event_mtd_form_idx",
                table: "mtd_event_subscribe",
                column: "mtd_form_id");

            migrationBuilder.CreateIndex(
                name: "id_unique",
                table: "mtd_event_subscribe",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_id",
                table: "mtd_event_subscribe",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_event_subscribe");

            migrationBuilder.DropColumn(
                name: "responsibility",
                table: "mtd_policy_forms");
        }
    }
}
