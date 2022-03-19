using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ToysDB.Models
{
    public partial class ToysContext : DbContext
    {
        public ToysContext()
        {
        }

        public ToysContext(DbContextOptions<ToysContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TableLogin> TableLogins { get; set; }
        public virtual DbSet<Бюджет> Бюджетs { get; set; }
        public virtual DbSet<ГотоваяПродукция> ГотоваяПродукцияs { get; set; }
        public virtual DbSet<Должности> Должностиs { get; set; }
        public virtual DbSet<ЕдиницыИзмерения> ЕдиницыИзмеренияs { get; set; }
        public virtual DbSet<ЗакупкаСырья> ЗакупкаСырьяs { get; set; }
        public virtual DbSet<Ингредиенты> Ингредиентыs { get; set; }
        public virtual DbSet<ПродажаПродукции> ПродажаПродукцииs { get; set; }
        public virtual DbSet<Производство> Производствоs { get; set; }
        public virtual DbSet<Сотрудники> Сотрудникиs { get; set; }
        public virtual DbSet<Сырьё> Сырьёs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
             //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-0NMHJ2G\\SQLEXPRESS;Database=Toys;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<TableLogin>(entity =>
            {
                entity.ToTable("table_login");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserLogin)
                    .HasMaxLength(10)
                    .HasColumnName("user_login")
                    .IsFixedLength(true);

                entity.Property(e => e.UserPassword)
                    .HasMaxLength(10)
                    .HasColumnName("user_password")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Бюджет>(entity =>
            {
                entity.ToTable("Бюджет");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Сумма)
                .HasColumnType("money")
                .HasColumnName("Сумма");
                entity.Property(e => e.Процент)
                .HasColumnType("tinyint")
                .HasColumnName("Процент");
                entity.Property(e => e.Бонус)
                .HasColumnType("money")
                .HasColumnName("Бонус");

            });

            modelBuilder.Entity<ГотоваяПродукция>(entity =>
            {
                entity.ToTable("Готовая продукция");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.ЕдиницаИзмерения).HasColumnName("Единица измерения");

                entity.Property(e => e.Наименование)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Сумма).HasColumnType("money");

                entity.HasOne(d => d.ЕдиницаИзмеренияNavigation)
                    .WithMany(p => p.ГотоваяПродукцияs)
                    .HasForeignKey(d => d.ЕдиницаИзмерения)
                    .HasConstraintName("FK_Готовая продукция_Единицы измерения");
            });

            modelBuilder.Entity<Должности>(entity =>
            {
                entity.ToTable("Должности");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Должность)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ЕдиницыИзмерения>(entity =>
            {
                entity.ToTable("Единицы измерения");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Наименование)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ЗакупкаСырья>(entity =>
            {
                entity.ToTable("Закупка сырья");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Дата).HasColumnType("date");

                entity.Property(e => e.Сумма).HasColumnType("money");

                entity.HasOne(d => d.СотрудникNavigation)
                    .WithMany(p => p.ЗакупкаСырьяs)
                    .HasForeignKey(d => d.Сотрудник)
                    .HasConstraintName("FK_Закупка сырья_Сотрудники");

                entity.HasOne(d => d.СырьёNavigation)
                    .WithMany(p => p.ЗакупкаСырьяs)
                    .HasForeignKey(d => d.Сырьё)
                    .HasConstraintName("FK_Закупка сырья_Сырьё");
            });

            modelBuilder.Entity<Ингредиенты>(entity =>
            {
                entity.ToTable("Ингредиенты");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.HasOne(d => d.ПродукцияNavigation)
                    .WithMany(p => p.Ингредиентыs)
                    .HasForeignKey(d => d.Продукция)
                    .HasConstraintName("FK_Ингредиенты_Готовая продукция");

                entity.HasOne(d => d.СырьеNavigation)
                    .WithMany(p => p.Ингредиентыs)
                    .HasForeignKey(d => d.Сырье)
                    .HasConstraintName("FK_Ингредиенты_Сырьё");
            });

            modelBuilder.Entity<ПродажаПродукции>(entity =>
            {
                entity.ToTable("Продажа продукции");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Дата).HasColumnType("date");

                entity.Property(e => e.Сумма).HasColumnType("money");

                entity.HasOne(d => d.ПродукцияNavigation)
                    .WithMany(p => p.ПродажаПродукцииs)
                    .HasForeignKey(d => d.Продукция)
                    .HasConstraintName("FK_Продажа продукции_Готовая продукция");

                entity.HasOne(d => d.СотрудникNavigation)
                    .WithMany(p => p.ПродажаПродукцииs)
                    .HasForeignKey(d => d.Сотрудник)
                    .HasConstraintName("FK_Продажа продукции_Сотрудники");
            });

            modelBuilder.Entity<Производство>(entity =>
            {
                entity.ToTable("Производство");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID")
                    .IsFixedLength(true);

                entity.Property(e => e.Дата).HasColumnType("date");

                entity.HasOne(d => d.ПродукцияNavigation)
                    .WithMany(p => p.Производствоs)
                    .HasForeignKey(d => d.Продукция)
                    .HasConstraintName("FK_Производство_Готовая продукция");

                entity.HasOne(d => d.СотрудникNavigation)
                    .WithMany(p => p.Производствоs)
                    .HasForeignKey(d => d.Сотрудник)
                    .HasConstraintName("FK_Производство_Сотрудники");
            });

            modelBuilder.Entity<Сотрудники>(entity =>
            {
                entity.ToTable("Сотрудники");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Адрес)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Оклад).HasColumnType("money");

                entity.Property(e => e.Телефон)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Фио)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ФИО");

                entity.HasOne(d => d.ДолжностьNavigation)
                    .WithMany(p => p.Сотрудникиs)
                    .HasForeignKey(d => d.Должность)
                    .HasConstraintName("FK_Сотрудники_Должности");
            });

            modelBuilder.Entity<Сырьё>(entity =>
            {
                entity.ToTable("Сырьё");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.ЕдиницаИзмерения).HasColumnName("Единица измерения");

                entity.Property(e => e.Наименование)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Сумма).HasColumnType("money");

                entity.HasOne(d => d.ЕдиницаИзмеренияNavigation)
                    .WithMany(p => p.Сырьёs)
                    .HasForeignKey(d => d.ЕдиницаИзмерения)
                    .HasConstraintName("FK_Сырьё_Единицы измерения");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
