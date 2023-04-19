﻿// <auto-generated />
using System;
using HelpDeskSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    [DbContext(typeof(EF_DataContext))]
    [Migration("20230415025753_My3Mirgration")]
    partial class My3Mirgration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("HelpDeskSystem.Models.Account", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("company")
                        .HasColumnType("text");

                    b.Property<string>("fullname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .HasColumnType("text");

                    b.Property<string>("test")
                        .HasColumnType("text");

                    b.Property<string>("test1")
                        .HasColumnType("text");

                    b.Property<string>("workemail")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Order", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("address")
                        .HasColumnType("text");

                    b.Property<DateTime>("createdon")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("phone")
                        .HasColumnType("text");

                    b.Property<int>("product_id")
                        .HasColumnType("integer");

                    b.Property<int>("productid")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("productid");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Product", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("brand")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("price")
                        .HasColumnType("numeric");

                    b.Property<string>("size")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Order", b =>
                {
                    b.HasOne("HelpDeskSystem.Models.Product", "product")
                        .WithMany()
                        .HasForeignKey("productid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("product");
                });
#pragma warning restore 612, 618
        }
    }
}
