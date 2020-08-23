using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "register_action",
                table: "mtd_store_stack_int",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "register_action",
                table: "mtd_store_stack_decimal",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "mtd_register",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    parent_limit = table.Column<sbyte>(type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_register", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_register_field",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_register_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    income = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    expense = table.Column<sbyte>(type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_register_field", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_form_register_field",
                        column: x => x.id,
                        principalTable: "mtd_form_part_field",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mtd_form_register",
                        column: x => x.mtd_register_id,
                        principalTable: "mtd_register",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_register",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_register_field",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_register_idx",
                table: "mtd_register_field",
                column: "mtd_register_id");


            migrationBuilder.Sql("update mtd_store_stack_int set register_action=0 where id<>''");
            migrationBuilder.Sql("update mtd_store_stack_decimal set register_action=0 where id<>''");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_register_field");

            migrationBuilder.DropTable(
                name: "mtd_register");

            migrationBuilder.DropColumn(
                name: "register_action",
                table: "mtd_store_stack_int");

            migrationBuilder.DropColumn(
                name: "register_action",
                table: "mtd_store_stack_decimal");
        }
    }
}
