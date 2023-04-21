﻿// <auto-generated />
using System;
using HelpDeskSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    [DbContext(typeof(EF_DataContext))]
    partial class EF_DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("workemail")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.ConfigMail", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<string>("incoming")
                        .HasColumnType("text");

                    b.Property<int?>("incomingPort")
                        .HasColumnType("integer");

                    b.Property<string>("outgoing")
                        .HasColumnType("text");

                    b.Property<int?>("outgoingPort")
                        .HasColumnType("integer");

                    b.Property<string>("password")
                        .HasColumnType("text");

                    b.Property<string>("yourName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("ConfigMails");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Contact", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("bio")
                        .HasColumnType("text");

                    b.Property<string>("city")
                        .HasColumnType("text");

                    b.Property<string>("company")
                        .HasColumnType("text");

                    b.Property<int?>("country")
                        .HasColumnType("integer");

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<string>("facebook")
                        .HasColumnType("text");

                    b.Property<string>("fullname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("github")
                        .HasColumnType("text");

                    b.Property<string>("linkedin")
                        .HasColumnType("text");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("twitter")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Country", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Countrys");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.EmailInfo", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("bcc")
                        .HasColumnType("text");

                    b.Property<string>("cc")
                        .HasColumnType("text");

                    b.Property<DateTime?>("date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("from")
                        .HasColumnType("text");

                    b.Property<int?>("idConfigEmail")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<string>("messageId")
                        .HasColumnType("text");

                    b.Property<string>("subject")
                        .HasColumnType("text");

                    b.Property<string>("textBody")
                        .HasColumnType("text");

                    b.Property<string>("to")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("EmailInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
