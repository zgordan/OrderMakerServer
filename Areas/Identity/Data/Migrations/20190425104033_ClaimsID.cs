using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Data.Migrations
{
    public partial class ClaimsID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.CreateIndex(                
               name: "IX_Unique_ID",
               table: "aspnetuserclaims",
               column: "Id",
               unique: true );

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "aspnetuserclaims",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);           
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Unique_ID", table: "aspnetuserclaims");            
            migrationBuilder.AlterColumn<int>(
                    name: "Id",
                    table: "aspnetuserclaims",
                    nullable: false,
                    oldClrType: typeof(int));
        }
    }
}
