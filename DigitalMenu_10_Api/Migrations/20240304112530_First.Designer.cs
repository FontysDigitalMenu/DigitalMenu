﻿// <auto-generated />
using System;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DigitalMenu_10_Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240304112530_First")]
    partial class First
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("MenuItemId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("MenuItemId");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.MenuItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.MenuItemCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("MenuItemId")
                        .HasColumnType("int");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MenuItemId");

                    b.ToTable("MenuItemCategories");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("PaymentStatus")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TableId")
                        .HasColumnType("int");

                    b.Property<int>("TotalAmount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TableId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Table", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("QRCode")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Tables");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Ingredient", b =>
                {
                    b.HasOne("DigitalMenu_20_BLL.Models.MenuItem", "MenuItem")
                        .WithMany("Ingredients")
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.MenuItem", b =>
                {
                    b.HasOne("DigitalMenu_20_BLL.Models.Order", "Order")
                        .WithMany("MenuItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.MenuItemCategory", b =>
                {
                    b.HasOne("DigitalMenu_20_BLL.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DigitalMenu_20_BLL.Models.MenuItem", "MenuItem")
                        .WithMany()
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Order", b =>
                {
                    b.HasOne("DigitalMenu_20_BLL.Models.Table", "Table")
                        .WithMany("Orders")
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Table");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.MenuItem", b =>
                {
                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Order", b =>
                {
                    b.Navigation("MenuItems");
                });

            modelBuilder.Entity("DigitalMenu_20_BLL.Models.Table", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
