﻿// <auto-generated />
using System;
using ASC.Core.Common.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ASC.Core.Common.Migrations.MSSql.WebstudioDbContextMSSql
{
    [DbContext(typeof(MSSqlWebstudioDbContext))]
    partial class MSSqlWebstudioDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioIndex", b =>
                {
                    b.Property<string>("IndexName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("index_name")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("last_modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("IndexName")
                        .HasName("webstudio_index_pkey");

                    b.ToTable("webstudio_index");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioSettings", b =>
                {
                    b.Property<int>("TenantId")
                        .HasColumnType("int")
                        .HasColumnName("TenantID");

                    b.Property<Guid>("Id")
                        .HasMaxLength(64)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ID");

                    b.Property<Guid>("UserId")
                        .HasMaxLength(64)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.HasKey("TenantId", "Id", "UserId")
                        .HasName("webstudio_settings_pkey");

                    b.HasIndex("Id")
                        .HasDatabaseName("ID");

                    b.ToTable("webstudio_settings");

                    b.HasData(
                        new
                        {
                            TenantId = 1,
                            Id = new Guid("9a925891-1f92-4ed7-b277-d6f649739f06"),
                            UserId = new Guid("00000000-0000-0000-0000-000000000000"),
                            Data = "{\"Completed\":false}"
                        });
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioUserVisit", b =>
                {
                    b.Property<int>("TenantId")
                        .HasColumnType("int")
                        .HasColumnName("tenantid");

                    b.Property<DateTime>("VisitDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("visitdate");

                    b.Property<Guid>("ProductId")
                        .HasMaxLength(38)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("productid");

                    b.Property<Guid>("UserId")
                        .HasMaxLength(38)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("userid");

                    b.Property<DateTime>("FirstVisitTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("firstvisittime");

                    b.Property<DateTime>("LastVisitTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("lastvisittime");

                    b.Property<int>("VisitCount")
                        .HasColumnType("int")
                        .HasColumnName("visitcount");

                    b.HasKey("TenantId", "VisitDate", "ProductId", "UserId")
                        .HasName("webstudio_uservisit_pkey");

                    b.HasIndex("VisitDate")
                        .HasDatabaseName("visitdate");

                    b.ToTable("webstudio_uservisit");
                });
#pragma warning restore 612, 618
        }
    }
}
