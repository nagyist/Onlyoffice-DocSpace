// <auto-generated />
using System;
using ASC.Feed.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.FeedDb
{
    [DbContext(typeof(FeedDbContext))]
    partial class FeedDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbTenant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("alias");

                    b.Property<bool>("Calls")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasColumnName("calls")
                        .HasDefaultValueSql("true");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creationdatetime");

                    b.Property<int>("Industry")
                        .HasColumnType("integer")
                        .HasColumnName("industry");

                    b.Property<string>("Language")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .HasColumnType("character(10)")
                        .HasColumnName("language")
                        .HasDefaultValueSql("'en-US'")
                        .IsFixedLength();

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("MappedDomain")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("mappeddomain")
                        .HasDefaultValueSql("NULL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<Guid?>("OwnerId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id")
                        .HasDefaultValueSql("NULL");

                    b.Property<string>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38)
                        .HasColumnType("character varying(38)")
                        .HasColumnName("payment_id")
                        .HasDefaultValueSql("NULL");

                    b.Property<bool>("Spam")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasColumnName("spam")
                        .HasDefaultValueSql("true");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime?>("StatusChanged")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("statuschanged");

                    b.Property<string>("TimeZone")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("timezone")
                        .HasDefaultValueSql("NULL");

                    b.Property<int>("TrustedDomainsEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("trusteddomainsenabled")
                        .HasDefaultValueSql("1");

                    b.Property<string>("TrustedDomainsRaw")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)")
                        .HasColumnName("trusteddomains")
                        .HasDefaultValueSql("NULL");

                    b.Property<int>("Version")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("version")
                        .HasDefaultValueSql("2");

                    b.Property<DateTime?>("Version_Changed")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("version_changed");

                    b.HasKey("Id");

                    b.HasIndex("Alias")
                        .IsUnique()
                        .HasDatabaseName("alias");

                    b.HasIndex("LastModified")
                        .HasDatabaseName("last_modified_tenants_tenants");

                    b.HasIndex("MappedDomain")
                        .HasDatabaseName("mappeddomain");

                    b.HasIndex("Version")
                        .HasDatabaseName("version");

                    b.ToTable("tenants_tenants", "onlyoffice");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Alias = "localhost",
                            Calls = false,
                            CreationDateTime = new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317),
                            Industry = 0,
                            LastModified = new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Web Office",
                            OwnerId = new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"),
                            Spam = false,
                            Status = 0,
                            TrustedDomainsEnabled = 0,
                            Version = 0
                        },
                        new
                        {
                            Id = -1,
                            Alias = "settings",
                            Calls = false,
                            CreationDateTime = new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317),
                            Industry = 0,
                            LastModified = new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Web Office",
                            OwnerId = new Guid("00000000-0000-0000-0000-000000000000"),
                            Spam = false,
                            Status = 1,
                            TrustedDomainsEnabled = 0,
                            Version = 0
                        });
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedAggregate", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(88)
                        .HasColumnType("character varying(88)")
                        .HasColumnName("id");

                    b.Property<DateTime>("AggregateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("aggregated_date");

                    b.Property<Guid>("Author")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("author")
                        .IsFixedLength();

                    b.Property<string>("ContextId")
                        .HasColumnType("text")
                        .HasColumnName("context_id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(70)
                        .HasColumnType("character varying(70)")
                        .HasColumnName("group_id")
                        .HasDefaultValueSql("NULL");

                    b.Property<string>("Json")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("json");

                    b.Property<string>("Keywords")
                        .HasColumnType("text")
                        .HasColumnName("keywords");

                    b.Property<Guid>("ModifiedBy")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("modified_by")
                        .IsFixedLength();

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modified_date");

                    b.Property<string>("Module")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("module");

                    b.Property<string>("Product")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("product");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer")
                        .HasColumnName("tenant");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "AggregateDate")
                        .HasDatabaseName("aggregated_date");

                    b.HasIndex("TenantId", "ModifiedDate")
                        .HasDatabaseName("modified_date");

                    b.HasIndex("TenantId", "Product")
                        .HasDatabaseName("product");

                    b.ToTable("feed_aggregate", "onlyoffice");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedLast", b =>
                {
                    b.Property<string>("LastKey")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("last_key");

                    b.Property<DateTime>("LastDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_date");

                    b.HasKey("LastKey")
                        .HasName("feed_last_pkey");

                    b.ToTable("feed_last", "onlyoffice");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedReaded", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer")
                        .HasColumnName("tenant_id");

                    b.Property<string>("Module")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("module");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasKey("UserId", "TenantId", "Module")
                        .HasName("feed_readed_pkey");

                    b.HasIndex("TenantId");

                    b.ToTable("feed_readed", "onlyoffice");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedUsers", b =>
                {
                    b.Property<string>("FeedId")
                        .HasMaxLength(88)
                        .HasColumnType("character varying(88)")
                        .HasColumnName("feed_id");

                    b.Property<Guid>("UserId")
                        .HasMaxLength(38)
                        .HasColumnType("uuid")
                        .HasColumnName("user_id")
                        .IsFixedLength();

                    b.HasKey("FeedId", "UserId")
                        .HasName("feed_users_pkey");

                    b.HasIndex("UserId")
                        .HasDatabaseName("user_id_feed_users");

                    b.ToTable("feed_users", "onlyoffice");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedAggregate", b =>
                {
                    b.HasOne("ASC.Core.Common.EF.Model.DbTenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedReaded", b =>
                {
                    b.HasOne("ASC.Core.Common.EF.Model.DbTenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ASC.Feed.Model.FeedUsers", b =>
                {
                    b.HasOne("ASC.Feed.Model.FeedAggregate", "Feed")
                        .WithMany()
                        .HasForeignKey("FeedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feed");
                });
#pragma warning restore 612, 618
        }
    }
}
