﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToysDB.Models;

namespace ToysDB.Migrations
{
    [DbContext(typeof(ToysContext))]
    [Migration("20220315192729_1")]
    partial class _1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.14")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ToysDB.Models.TableLogin", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint")
                        .HasColumnName("id");

                    b.Property<string>("UserLogin")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .HasColumnName("user_login")
                        .IsFixedLength(true);

                    b.Property<string>("UserPassword")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .HasColumnName("user_password")
                        .IsFixedLength(true);

                    b.HasKey("Id");

                    b.ToTable("table_login");
                });

            modelBuilder.Entity("ToysDB.Models.Бюджет", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("Сумма")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.ToTable("Бюджет");
                });

            modelBuilder.Entity("ToysDB.Models.ГотоваяПродукция", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte?>("ЕдиницаИзмерения")
                        .HasColumnType("tinyint")
                        .HasColumnName("Единица измерения");

                    b.Property<short?>("Количество")
                        .HasColumnType("smallint");

                    b.Property<string>("Наименование")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal?>("Сумма")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.HasIndex("ЕдиницаИзмерения");

                    b.ToTable("Готовая продукция");
                });

            modelBuilder.Entity("ToysDB.Models.Должности", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Должность")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Должности");
                });

            modelBuilder.Entity("ToysDB.Models.ЕдиницыИзмерения", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Наименование")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Единицы измерения");
                });

            modelBuilder.Entity("ToysDB.Models.ЗакупкаСырья", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Дата")
                        .HasColumnType("date");

                    b.Property<short?>("Количество")
                        .HasColumnType("smallint");

                    b.Property<byte?>("Сотрудник")
                        .HasColumnType("tinyint");

                    b.Property<decimal?>("Сумма")
                        .HasColumnType("money");

                    b.Property<byte?>("Сырьё")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("Сотрудник");

                    b.HasIndex("Сырьё");

                    b.ToTable("Закупка сырья");
                });

            modelBuilder.Entity("ToysDB.Models.Ингредиенты", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short?>("Количество")
                        .HasColumnType("smallint");

                    b.Property<byte?>("Продукция")
                        .HasColumnType("tinyint");

                    b.Property<byte?>("Сырье")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("Продукция");

                    b.HasIndex("Сырье");

                    b.ToTable("Ингредиенты");
                });

            modelBuilder.Entity("ToysDB.Models.ПродажаПродукции", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Дата")
                        .HasColumnType("date");

                    b.Property<float?>("Количество")
                        .HasColumnType("real");

                    b.Property<byte?>("Продукция")
                        .HasColumnType("tinyint");

                    b.Property<byte?>("Сотрудник")
                        .HasColumnType("tinyint");

                    b.Property<decimal?>("Сумма")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.HasIndex("Продукция");

                    b.HasIndex("Сотрудник");

                    b.ToTable("Продажа продукции");
                });

            modelBuilder.Entity("ToysDB.Models.Производство", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .HasColumnName("ID")
                        .IsFixedLength(true);

                    b.Property<DateTime?>("Дата")
                        .HasColumnType("date");

                    b.Property<short?>("Количество")
                        .HasColumnType("smallint");

                    b.Property<byte?>("Продукция")
                        .HasColumnType("tinyint");

                    b.Property<byte?>("Сотрудник")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("Продукция");

                    b.HasIndex("Сотрудник");

                    b.ToTable("Производство");
                });

            modelBuilder.Entity("ToysDB.Models.Сотрудники", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Адрес")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<byte?>("Должность")
                        .HasColumnType("tinyint");

                    b.Property<decimal?>("Оклад")
                        .HasColumnType("money");

                    b.Property<string>("Телефон")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Фио")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("ФИО");

                    b.HasKey("Id");

                    b.HasIndex("Должность");

                    b.ToTable("Сотрудники");
                });

            modelBuilder.Entity("ToysDB.Models.Сырьё", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte?>("ЕдиницаИзмерения")
                        .HasColumnType("tinyint")
                        .HasColumnName("Единица измерения");

                    b.Property<short?>("Количество")
                        .HasColumnType("smallint");

                    b.Property<string>("Наименование")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal?>("Сумма")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.HasIndex("ЕдиницаИзмерения");

                    b.ToTable("Сырьё");
                });

            modelBuilder.Entity("ToysDB.Models.ГотоваяПродукция", b =>
                {
                    b.HasOne("ToysDB.Models.ЕдиницыИзмерения", "ЕдиницаИзмеренияNavigation")
                        .WithMany("ГотоваяПродукцияs")
                        .HasForeignKey("ЕдиницаИзмерения")
                        .HasConstraintName("FK_Готовая продукция_Единицы измерения");

                    b.Navigation("ЕдиницаИзмеренияNavigation");
                });

            modelBuilder.Entity("ToysDB.Models.ЗакупкаСырья", b =>
                {
                    b.HasOne("ToysDB.Models.Сотрудники", "СотрудникNavigation")
                        .WithMany("ЗакупкаСырьяs")
                        .HasForeignKey("Сотрудник")
                        .HasConstraintName("FK_Закупка сырья_Сотрудники");

                    b.HasOne("ToysDB.Models.Сырьё", "СырьёNavigation")
                        .WithMany("ЗакупкаСырьяs")
                        .HasForeignKey("Сырьё")
                        .HasConstraintName("FK_Закупка сырья_Сырьё");

                    b.Navigation("СотрудникNavigation");

                    b.Navigation("СырьёNavigation");
                });

            modelBuilder.Entity("ToysDB.Models.Ингредиенты", b =>
                {
                    b.HasOne("ToysDB.Models.ГотоваяПродукция", "ПродукцияNavigation")
                        .WithMany("Ингредиентыs")
                        .HasForeignKey("Продукция")
                        .HasConstraintName("FK_Ингредиенты_Готовая продукция");

                    b.HasOne("ToysDB.Models.Сырьё", "СырьеNavigation")
                        .WithMany("Ингредиентыs")
                        .HasForeignKey("Сырье")
                        .HasConstraintName("FK_Ингредиенты_Сырьё");

                    b.Navigation("ПродукцияNavigation");

                    b.Navigation("СырьеNavigation");
                });

            modelBuilder.Entity("ToysDB.Models.ПродажаПродукции", b =>
                {
                    b.HasOne("ToysDB.Models.ГотоваяПродукция", "ПродукцияNavigation")
                        .WithMany("ПродажаПродукцииs")
                        .HasForeignKey("Продукция")
                        .HasConstraintName("FK_Продажа продукции_Готовая продукция");

                    b.HasOne("ToysDB.Models.Сотрудники", "СотрудникNavigation")
                        .WithMany("ПродажаПродукцииs")
                        .HasForeignKey("Сотрудник")
                        .HasConstraintName("FK_Продажа продукции_Сотрудники");

                    b.Navigation("ПродукцияNavigation");

                    b.Navigation("СотрудникNavigation");
                });

            modelBuilder.Entity("ToysDB.Models.Производство", b =>
                {
                    b.HasOne("ToysDB.Models.ГотоваяПродукция", "ПродукцияNavigation")
                        .WithMany("Производствоs")
                        .HasForeignKey("Продукция")
                        .HasConstraintName("FK_Производство_Готовая продукция");

                    b.HasOne("ToysDB.Models.Сотрудники", "СотрудникNavigation")
                        .WithMany("Производствоs")
                        .HasForeignKey("Сотрудник")
                        .HasConstraintName("FK_Производство_Сотрудники");

                    b.Navigation("ПродукцияNavigation");

                    b.Navigation("СотрудникNavigation");
                });

            modelBuilder.Entity("ToysDB.Models.Сотрудники", b =>
                {
                    b.HasOne("ToysDB.Models.Должности", "ДолжностьNavigation")
                        .WithMany("Сотрудникиs")
                        .HasForeignKey("Должность")
                        .HasConstraintName("FK_Сотрудники_Должности");

                    b.Navigation("ДолжностьNavigation");
                });

            modelBuilder.Entity("ToysDB.Models.Сырьё", b =>
                {
                    b.HasOne("ToysDB.Models.ЕдиницыИзмерения", "ЕдиницаИзмеренияNavigation")
                        .WithMany("Сырьёs")
                        .HasForeignKey("ЕдиницаИзмерения")
                        .HasConstraintName("FK_Сырьё_Единицы измерения");

                    b.Navigation("ЕдиницаИзмеренияNavigation");
                });

            modelBuilder.Entity("ToysDB.Models.ГотоваяПродукция", b =>
                {
                    b.Navigation("Ингредиентыs");

                    b.Navigation("ПродажаПродукцииs");

                    b.Navigation("Производствоs");
                });

            modelBuilder.Entity("ToysDB.Models.Должности", b =>
                {
                    b.Navigation("Сотрудникиs");
                });

            modelBuilder.Entity("ToysDB.Models.ЕдиницыИзмерения", b =>
                {
                    b.Navigation("ГотоваяПродукцияs");

                    b.Navigation("Сырьёs");
                });

            modelBuilder.Entity("ToysDB.Models.Сотрудники", b =>
                {
                    b.Navigation("ЗакупкаСырьяs");

                    b.Navigation("ПродажаПродукцииs");

                    b.Navigation("Производствоs");
                });

            modelBuilder.Entity("ToysDB.Models.Сырьё", b =>
                {
                    b.Navigation("ЗакупкаСырьяs");

                    b.Navigation("Ингредиентыs");
                });
#pragma warning restore 612, 618
        }
    }
}
