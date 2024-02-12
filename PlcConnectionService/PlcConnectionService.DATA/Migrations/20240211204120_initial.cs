using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlcConnectionService.DATA.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlcDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Counter = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<int>(type: "int", nullable: false),
                    TN = table.Column<int>(type: "int", nullable: false),
                    AdimNo = table.Column<int>(type: "int", nullable: false),
                    SiloNo = table.Column<int>(type: "int", nullable: false),
                    ReceteID = table.Column<int>(type: "int", nullable: false),
                    PartiID = table.Column<int>(type: "int", nullable: false),
                    HammaddeID = table.Column<int>(type: "int", nullable: false),
                    Alinacak = table.Column<int>(type: "int", nullable: false),
                    Alinan = table.Column<int>(type: "int", nullable: false),
                    Shut = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlcDatas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlcDatas");
        }
    }
}
