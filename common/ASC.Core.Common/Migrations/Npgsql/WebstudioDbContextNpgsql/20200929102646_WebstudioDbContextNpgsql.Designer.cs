﻿// <auto-generated />
using System;
using ASC.Core.Common.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.WebstudioDbContextNpgsql
{
    [DbContext(typeof(WebstudioDbContext))]
    [Migration("20200929102646_WebstudioDbContextNpgsql")]
    partial class WebstudioDbContextNpgsql
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioIndex", b =>
                {
                    b.Property<string>("IndexName")
                        .HasColumnName("index_name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("last_modified")
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("IndexName")
                        .HasName("webstudio_index_pkey");

                    b.ToTable("webstudio_index","onlyoffice");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioSettings", b =>
                {
                    b.Property<int>("TenantId")
                        .HasColumnName("TenantID")
                        .HasColumnType("integer");

                    b.Property<Guid>("Id")
                        .HasColumnName("ID")
                        .HasColumnType("uuid")
                        .HasMaxLength(64);

                    b.Property<Guid>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("uuid")
                        .HasMaxLength(64);

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TenantId", "Id", "UserId")
                        .HasName("webstudio_settings_pkey");

                    b.HasIndex("Id")
                        .HasName("ID");

                    b.ToTable("webstudio_settings","onlyoffice");

                    b.HasData(
                        new
                        {
                            TenantId = 1,
                            Id = new Guid("9a925891-1f92-4ed7-b277-d6f649739f06"),
                            UserId = new Guid("00000000-0000-0000-0000-000000000000"),
                            Data = "{'Completed':false}"
                        });
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioUserVisit", b =>
                {
                    b.Property<int>("TenantId")
                        .HasColumnName("tenantid")
                        .HasColumnType("integer");

                    b.Property<DateTime>("VisitDate")
                        .HasColumnName("visitdate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("ProductId")
                        .HasColumnName("productid")
                        .HasColumnType("uuid")
                        .HasMaxLength(38);

                    b.Property<Guid>("UserId")
                        .HasColumnName("userid")
                        .HasColumnType("uuid")
                        .HasMaxLength(38);

                    b.Property<DateTime>("FirstVisitTime")
                        .HasColumnName("firstvisittime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LastVisitTime")
                        .HasColumnName("lastvisittime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("VisitCount")
                        .HasColumnName("visitcount")
                        .HasColumnType("integer");

                    b.HasKey("TenantId", "VisitDate", "ProductId", "UserId")
                        .HasName("webstudio_uservisit_pkey");

                    b.HasIndex("VisitDate")
                        .HasName("visitdate");

                    b.ToTable("webstudio_uservisit","onlyoffice");
                });
#pragma warning restore 612, 618
        }
    }
}
