/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class StoreParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Parent",
                table: "mtd_store",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mtd_store_Parent",
                table: "mtd_store",
                column: "Parent");

            migrationBuilder.AddForeignKey(
                name: "fk_mtd_store_parent",
                table: "mtd_store",
                column: "Parent",
                principalTable: "mtd_store",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mtd_store_parent",
                table: "mtd_store");

            migrationBuilder.DropIndex(
                name: "IX_mtd_store_Parent",
                table: "mtd_store");

            migrationBuilder.DropColumn(
                name: "Parent",
                table: "mtd_store");
        }
    }
}
