﻿// <auto-generated />
using System;
using ASC.Core.Common.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ASC.Core.Common.Migrations.MSSql.FeedDbContextMSSql
{
    [DbContext(typeof(MSSqlFeedDbContext))]
    [Migration("20211025165246_MSSqlFeedDbContext_Init")]
    partial class MSSqlFeedDbContext_Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ASC.Core.Common.EF.Model.FeedAggregate", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(88)
                        .HasColumnType("nvarchar(88)")
                        .HasColumnName("id")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<DateTime>("AggregateDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("aggregated_date");

                    b.Property<Guid>("Author")
                        .HasMaxLength(38)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("author")
                        .IsFixedLength(true);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_date");

                    b.Property<string>("GroupId")
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)")
                        .HasColumnName("group_id")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<string>("Json")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("json")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<string>("Keywords")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("keywords")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<Guid>("ModifiedBy")
                        .HasMaxLength(38)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("modified_by")
                        .IsFixedLength(true);

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("modified_date");

                    b.Property<string>("Module")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("module")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<string>("Product")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("product")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<int>("Tenant")
                        .HasColumnType("int")
                        .HasColumnName("tenant");

                    b.HasKey("Id");

                    b.HasIndex("Tenant", "AggregateDate")
                        .HasDatabaseName("aggregated_date");

                    b.HasIndex("Tenant", "ModifiedDate")
                        .HasDatabaseName("modified_date");

                    b.HasIndex("Tenant", "Product")
                        .HasDatabaseName("product");

                    b.ToTable("feed_aggregate");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.FeedLast", b =>
                {
                    b.Property<string>("LastKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("last_key")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<DateTime>("LastDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("last_date");

                    b.HasKey("LastKey")
                        .HasName("feed_last_pkey");

                    b.ToTable("feed_last");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.FeedReaded", b =>
                {
                    b.Property<int>("Tenant")
                        .HasColumnType("int")
                        .HasColumnName("tenant_id");

                    b.Property<Guid>("UserId")
                        .HasMaxLength(38)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("user_id");

                    b.Property<string>("Module")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("module")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2")
                        .HasColumnName("timestamp");

                    b.HasKey("Tenant", "UserId", "Module")
                        .HasName("feed_readed_pkey");

                    b.ToTable("feed_readed");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.FeedUsers", b =>
                {
                    b.Property<string>("FeedId")
                        .HasMaxLength(88)
                        .HasColumnType("nvarchar(88)")
                        .HasColumnName("feed_id")
                        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8");

                    b.Property<Guid>("UserId")
                        .HasMaxLength(38)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("user_id")
                        .IsFixedLength(true);

                    b.HasKey("FeedId", "UserId")
                        .HasName("feed_users_pkey");

                    b.HasIndex("UserId")
                        .HasDatabaseName("user_id_feed_users");

                    b.ToTable("feed_users");
                });
#pragma warning restore 612, 618
        }
    }
}
