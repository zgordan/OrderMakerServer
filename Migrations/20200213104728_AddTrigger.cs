using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class AddTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<string>(
                name: "mtd_sys_trigger",
                table: "mtd_form_part_field",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "9C85B07F-9236-4314-A29E-87B20093CF82");

            migrationBuilder.CreateTable(
                name: "mtd_sys_trigger",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_sys_trigger", x => x.id);
                });

            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('9C85B07F-9236-4314-A29E-87B20093CF82','No Trigger',0)");      
            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('D3663BC7-FA05-4F64-8EBD-F25414E459B8','Datetime now',1)");
            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('33E8212E-059B-482D-8CBD-DFDB073E3B63','User group',2)");
            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('08FE6202-45D7-46C2-B343-B79FD4831F27','User name',3)");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_field_mtd_sys_trigger_idx",
                table: "mtd_form_part_field",
                column: "mtd_sys_trigger");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_sys_trigger",
                column: "id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_mtd_form_part_field_mtd_sys_trigger",
                table: "mtd_form_part_field",
                column: "mtd_sys_trigger",
                principalTable: "mtd_sys_trigger",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mtd_form_part_field_mtd_sys_trigger",
                table: "mtd_form_part_field");

            migrationBuilder.DropTable(
                name: "mtd_sys_trigger");

            migrationBuilder.DropIndex(
                name: "fk_mtd_form_part_field_mtd_sys_trigger_idx",
                table: "mtd_form_part_field");

            migrationBuilder.DropColumn(
                name: "mtd_sys_trigger",
                table: "mtd_form_part_field");
        }
    }
}
