using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Data.Migrations
{
    public partial class AddCpqRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO aspnetroles (Id,Name,NormalizedName,ConcurrencyStamp,Seq,Title) values ('4a6a64d7-f431-40b4-bf6b-0a4776a1b38f','cpq-salesmanager', 'CPQ-SALESMANAGER','5f5ec795-f9f1-4b34-ba82-177b5e6d9b29',20,'Sales manager')");
            migrationBuilder.Sql("INSERT INTO aspnetroles (Id,Name,NormalizedName,ConcurrencyStamp,Seq,Title) values ('7e9ac524-5370-4b9c-bd4e-a682e9772c17','cpq-guest', 'CPQ-GUEST','98ba1e43-fb83-469d-8439-88044a210b40',10,'Guest')");
            migrationBuilder.Sql("INSERT INTO aspnetroles (Id,Name,NormalizedName,ConcurrencyStamp,Seq,Title) values ('8bad5347-fc45-400c-9484-072e3082aa11','cpq-admin', 'CPQ-ADMIN','bfbfc9af-8023-48b8-a678-9177ab60cd35',50,'Administrator')");
            migrationBuilder.Sql("INSERT INTO aspnetroles (Id,Name,NormalizedName,ConcurrencyStamp,Seq,Title) values ('cee9e72f-5158-4945-ac45-ee06194ec1db','cpq-goodsmanager', 'CPQ-GOODSMANAGER','a02a0ed3-0f7f-4a0f-a713-d22123bd21b1',40,'Goods manager')");
            migrationBuilder.Sql("INSERT INTO aspnetroles (Id,Name,NormalizedName,ConcurrencyStamp,Seq,Title) values ('da70f6f6-69c3-485c-b6bb-bc6e4a6cb3f6','cpq-supervisor', 'CPQ-SUPERVISOR','45ad7a2f-b77c-41c1-b938-86fead76e82b',30,'Supervisor')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from aspnetroles where Id='4a6a64d7-f431-40b4-bf6b-0a4776a1b38f'");
            migrationBuilder.Sql("delete from aspnetroles where Id='7e9ac524-5370-4b9c-bd4e-a682e9772c17'");
            migrationBuilder.Sql("delete from aspnetroles where Id='8bad5347-fc45-400c-9484-072e3082aa11'");
            migrationBuilder.Sql("delete from aspnetroles where Id='cee9e72f-5158-4945-ac45-ee06194ec1db'");
            migrationBuilder.Sql("delete from aspnetroles where Id='da70f6f6-69c3-485c-b6bb-bc6e4a6cb3f6'");
        }
    }
}
