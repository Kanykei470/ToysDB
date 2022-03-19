using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ToysDB.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Бюджет",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Сумма = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Бюджет", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Должности",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Должность = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Должности", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Единицы измерения",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Наименование = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Единицы измерения", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "table_login",
                columns: table => new
                {
                    id = table.Column<byte>(type: "tinyint", nullable: false),
                    user_login = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    user_password = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_login", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Сотрудники",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ФИО = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Должность = table.Column<byte>(type: "tinyint", nullable: true),
                    Оклад = table.Column<decimal>(type: "money", nullable: true),
                    Адрес = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Телефон = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Сотрудники", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Сотрудники_Должности",
                        column: x => x.Должность,
                        principalTable: "Должности",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Готовая продукция",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Наименование = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Единицаизмерения = table.Column<byte>(name: "Единица измерения", type: "tinyint", nullable: true),
                    Сумма = table.Column<decimal>(type: "money", nullable: true),
                    Количество = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Готовая продукция", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Готовая продукция_Единицы измерения",
                        column: x => x.Единицаизмерения,
                        principalTable: "Единицы измерения",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Сырьё",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Наименование = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Единицаизмерения = table.Column<byte>(name: "Единица измерения", type: "tinyint", nullable: true),
                    Сумма = table.Column<decimal>(type: "money", nullable: true),
                    Количество = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Сырьё", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Сырьё_Единицы измерения",
                        column: x => x.Единицаизмерения,
                        principalTable: "Единицы измерения",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Продажа продукции",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Продукция = table.Column<byte>(type: "tinyint", nullable: true),
                    Количество = table.Column<float>(type: "real", nullable: true),
                    Сумма = table.Column<decimal>(type: "money", nullable: true),
                    Дата = table.Column<DateTime>(type: "date", nullable: true),
                    Сотрудник = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Продажа продукции", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Продажа продукции_Готовая продукция",
                        column: x => x.Продукция,
                        principalTable: "Готовая продукция",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Продажа продукции_Сотрудники",
                        column: x => x.Сотрудник,
                        principalTable: "Сотрудники",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Производство",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Продукция = table.Column<byte>(type: "tinyint", nullable: true),
                    Количество = table.Column<short>(type: "smallint", nullable: true),
                    Дата = table.Column<DateTime>(type: "date", nullable: true),
                    Сотрудник = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Производство", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Производство_Готовая продукция",
                        column: x => x.Продукция,
                        principalTable: "Готовая продукция",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Производство_Сотрудники",
                        column: x => x.Сотрудник,
                        principalTable: "Сотрудники",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Закупка сырья",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Сырьё = table.Column<byte>(type: "tinyint", nullable: true),
                    Количество = table.Column<short>(type: "smallint", nullable: true),
                    Сумма = table.Column<decimal>(type: "money", nullable: true),
                    Дата = table.Column<DateTime>(type: "date", nullable: true),
                    Сотрудник = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Закупка сырья", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Закупка сырья_Сотрудники",
                        column: x => x.Сотрудник,
                        principalTable: "Сотрудники",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Закупка сырья_Сырьё",
                        column: x => x.Сырьё,
                        principalTable: "Сырьё",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ингредиенты",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Продукция = table.Column<byte>(type: "tinyint", nullable: true),
                    Сырье = table.Column<byte>(type: "tinyint", nullable: true),
                    Количество = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ингредиенты", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ингредиенты_Готовая продукция",
                        column: x => x.Продукция,
                        principalTable: "Готовая продукция",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ингредиенты_Сырьё",
                        column: x => x.Сырье,
                        principalTable: "Сырьё",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Готовая продукция_Единица измерения",
                table: "Готовая продукция",
                column: "Единица измерения");

            migrationBuilder.CreateIndex(
                name: "IX_Закупка сырья_Сотрудник",
                table: "Закупка сырья",
                column: "Сотрудник");

            migrationBuilder.CreateIndex(
                name: "IX_Закупка сырья_Сырьё",
                table: "Закупка сырья",
                column: "Сырьё");

            migrationBuilder.CreateIndex(
                name: "IX_Ингредиенты_Продукция",
                table: "Ингредиенты",
                column: "Продукция");

            migrationBuilder.CreateIndex(
                name: "IX_Ингредиенты_Сырье",
                table: "Ингредиенты",
                column: "Сырье");

            migrationBuilder.CreateIndex(
                name: "IX_Продажа продукции_Продукция",
                table: "Продажа продукции",
                column: "Продукция");

            migrationBuilder.CreateIndex(
                name: "IX_Продажа продукции_Сотрудник",
                table: "Продажа продукции",
                column: "Сотрудник");

            migrationBuilder.CreateIndex(
                name: "IX_Производство_Продукция",
                table: "Производство",
                column: "Продукция");

            migrationBuilder.CreateIndex(
                name: "IX_Производство_Сотрудник",
                table: "Производство",
                column: "Сотрудник");

            migrationBuilder.CreateIndex(
                name: "IX_Сотрудники_Должность",
                table: "Сотрудники",
                column: "Должность");

            migrationBuilder.CreateIndex(
                name: "IX_Сырьё_Единица измерения",
                table: "Сырьё",
                column: "Единица измерения");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Бюджет");

            migrationBuilder.DropTable(
                name: "Закупка сырья");

            migrationBuilder.DropTable(
                name: "Ингредиенты");

            migrationBuilder.DropTable(
                name: "Продажа продукции");

            migrationBuilder.DropTable(
                name: "Производство");

            migrationBuilder.DropTable(
                name: "table_login");

            migrationBuilder.DropTable(
                name: "Сырьё");

            migrationBuilder.DropTable(
                name: "Готовая продукция");

            migrationBuilder.DropTable(
                name: "Сотрудники");

            migrationBuilder.DropTable(
                name: "Единицы измерения");

            migrationBuilder.DropTable(
                name: "Должности");
        }
    }
}
