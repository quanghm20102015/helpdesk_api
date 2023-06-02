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
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("HelpDeskSystem.Models.Account", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("avatar")
                        .HasColumnType("text");

                    b.Property<string>("company")
                        .HasColumnType("text");

                    b.Property<bool>("confirm")
                        .HasColumnType("boolean");

                    b.Property<string>("fileName")
                        .HasColumnType("text");

                    b.Property<string>("fullname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("idCompany")
                        .HasColumnType("integer");

                    b.Property<string>("idGuId")
                        .HasColumnType("text");

                    b.Property<bool>("isDelete")
                        .HasColumnType("boolean");

                    b.Property<bool>("login")
                        .HasColumnType("boolean");

                    b.Property<string>("password")
                        .HasColumnType("text");

                    b.Property<int?>("role")
                        .HasColumnType("integer");

                    b.Property<int>("status")
                        .HasColumnType("integer");

                    b.Property<string>("workemail")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Company", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("companyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Companys");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.ConfigMail", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<int>("idCompany")
                        .HasColumnType("integer");

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

                    b.Property<int>("idCompany")
                        .HasColumnType("integer");

                    b.Property<int?>("idLabel")
                        .HasColumnType("integer");

                    b.Property<string>("linkedin")
                        .HasColumnType("text");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("twitter")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.ContactLabel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int?>("idContact")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<int?>("idLabel")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("ContactLabels");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.ContactNote", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int?>("idContact")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<string>("note")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("ContactNotes");
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

            modelBuilder.Entity("HelpDeskSystem.Models.Csat", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<DateTime>("dateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("descriptionFeedBack")
                        .HasColumnType("text");

                    b.Property<int>("idCompany")
                        .HasColumnType("integer");

                    b.Property<int>("idFeedBack")
                        .HasColumnType("integer");

                    b.Property<string>("idGuIdEmailInfo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Csats");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.EmailInfo", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int?>("assign")
                        .HasColumnType("integer");

                    b.Property<string>("bcc")
                        .HasColumnType("text");

                    b.Property<string>("cc")
                        .HasColumnType("text");

                    b.Property<DateTime?>("date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("dateDelete")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("from")
                        .HasColumnType("text");

                    b.Property<string>("fromName")
                        .HasColumnType("text");

                    b.Property<int?>("idCompany")
                        .HasColumnType("integer");

                    b.Property<int?>("idConfigEmail")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<int>("idContact")
                        .HasColumnType("integer");

                    b.Property<string>("idGuId")
                        .HasColumnType("text");

                    b.Property<int?>("idLabel")
                        .HasColumnType("integer");

                    b.Property<string>("idReference")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("idUserDelete")
                        .HasColumnType("integer");

                    b.Property<bool>("isAssign")
                        .HasColumnType("boolean");

                    b.Property<bool>("isDelete")
                        .HasColumnType("boolean");

                    b.Property<bool>("mainConversation")
                        .HasColumnType("boolean");

                    b.Property<string>("messageId")
                        .HasColumnType("text");

                    b.Property<bool>("read")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("resolveTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("status")
                        .HasColumnType("integer");

                    b.Property<string>("subject")
                        .HasColumnType("text");

                    b.Property<string>("textBody")
                        .HasColumnType("text");

                    b.Property<string>("to")
                        .HasColumnType("text");

                    b.Property<int>("type")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("EmailInfos");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.EmailInfoAssign", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int?>("idEmailInfo")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<int?>("idUser")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("EmailInfoAssigns");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.EmailInfoAttach", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("extension")
                        .HasColumnType("text");

                    b.Property<string>("fileName")
                        .HasColumnType("text");

                    b.Property<int?>("idEmailInfo")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<string>("pathFile")
                        .HasColumnType("text");

                    b.Property<string>("sizeText")
                        .HasColumnType("text");

                    b.Property<string>("type")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("EmailInfoAttachs");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.EmailInfoFollow", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int?>("idEmailInfo")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<int?>("idUser")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("EmailInfoFollows");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.EmailInfoLabel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int?>("idEmailInfo")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<int?>("idLabel")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("EmailInfoLabels");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.History", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("content")
                        .HasColumnType("text");

                    b.Property<string>("fullName")
                        .HasColumnType("text");

                    b.Property<int?>("idCompany")
                        .HasColumnType("integer");

                    b.Property<int?>("idDetail")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("type")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("Historys");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Label", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("color")
                        .HasColumnType("text");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<int>("idCompany")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Status", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int>("sort")
                        .HasColumnType("integer");

                    b.Property<string>("statusName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.Team", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<int>("idCompany")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("HelpDeskSystem.Models.TeamAgent", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int>("idAgent")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("TeamAgents");
                });
#pragma warning restore 612, 618
        }
    }
}
