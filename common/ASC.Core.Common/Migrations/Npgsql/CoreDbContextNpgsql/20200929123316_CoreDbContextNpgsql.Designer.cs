﻿// <auto-generated />
using System;
using ASC.Core.Common.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.CoreDbContextNpgsql
{
    [DbContext(typeof(CoreDbContext))]
    [Migration("20200929123316_CoreDbContextNpgsql")]
    partial class CoreDbContextNpgsql
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ASC.Core.Common.EF.Acl", b =>
                {
                    b.Property<int>("Tenant")
                        .HasColumnName("tenant")
                        .HasColumnType("integer");

                    b.Property<Guid>("Subject")
                        .HasColumnName("subject")
                        .HasColumnType("uuid")
                        .HasMaxLength(38);

                    b.Property<Guid>("Action")
                        .HasColumnName("action")
                        .HasColumnType("uuid")
                        .HasMaxLength(38);

                    b.Property<string>("Object")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("object")
                        .HasColumnType("character varying(255)")
                        .HasDefaultValueSql("''")
                        .HasMaxLength(255);

                    b.Property<int>("AceType")
                        .HasColumnName("acetype")
                        .HasColumnType("integer");

                    b.HasKey("Tenant", "Subject", "Action", "Object")
                        .HasName("core_acl_pkey");

                    b.ToTable("core_acl","onlyoffice");

                    b.HasData(
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"),
                            Action = new Guid("ef5e6790-f346-4b6e-b662-722bc28cb0db"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"),
                            Action = new Guid("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("088d5940-a80f-4403-9741-d610718ce95c"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("08d66144-e1c9-4065-9aa1-aa4bba0a7bc8"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("08d75c97-cf3f-494b-90d1-751c941fe2dd"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("0d1f72a8-63da-47ea-ae42-0900e4ac72a9"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("13e30b51-5b4d-40a5-8575-cb561899eeb1"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("19f658ae-722b-4cd8-8236-3ad150801d96"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("2c6552b3-b2e0-4a00-b8fd-13c161e337b1"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("388c29d3-c662-4a61-bf47-fc2f7094224a"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("40bf31f4-3132-4e76-8d5c-9828a89501a3"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("49ae8915-2b30-4348-ab74-b152279364fb"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("9018c001-24c2-44bf-a1db-d1121a570e74"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("948ad738-434b-4a88-8e38-7569d332910a"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("9d75a568-52aa-49d8-ad43-473756cd8903"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("a362fe79-684e-4d43-a599-65bc1f4e167f"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("c426c349-9ad4-47cd-9b8f-99fc30675951"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("d11ebcb9-0e6e-45e6-a6d0-99c41d687598"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("d1f3b53d-d9e2-4259-80e7-d24380978395"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("d49f4e30-da10-4b39-bc6d-b41ef6e039d3"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"),
                            Action = new Guid("d852b66f-6719-45e1-8657-18f0bb791690"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("e0759a42-47f0-4763-a26a-d5aa665bec35"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("fbc37705-a04c-40ad-a68c-ce2f0423f397"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("fcac42b8-9386-48eb-a938-d19b3c576912"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("13e30b51-5b4d-40a5-8575-cb561899eeb1"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("49ae8915-2b30-4348-ab74-b152279364fb"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("9018c001-24c2-44bf-a1db-d1121a570e74"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("d1f3b53d-d9e2-4259-80e7-d24380978395"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("e0759a42-47f0-4763-a26a-d5aa665bec35"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("f11e88d7-f185-4372-927c-d88008d2c483"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"),
                            Action = new Guid("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("00e7dfc5-ac49-4fd3-a1d6-98d84e877ac4"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("14be970f-7af5-4590-8e81-ea32b5f7866d"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("18ecc94d-6afa-4994-8406-aee9dff12ce2"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("298530eb-435e-4dc6-a776-9abcd95c70e9"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("430eaf70-1886-483c-a746-1a18e3e6bb63"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("557d6503-633b-4490-a14c-6473147ce2b3"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("724cbb75-d1c9-451e-bae0-4de0db96b1f7"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("7cb5c0d1-d254-433f-abe3-ff23373ec631"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("91b29dcd-9430-4403-b17a-27d09189be88"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("a18480a4-6d18-4c71-84fa-789888791f45"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("b630d29b-1844-4bda-bbbe-cf5542df3559"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("c62a9e8d-b24c-4513-90aa-7ff0f8ba38eb"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("d7cdb020-288b-41e5-a857-597347618533"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("662f3db7-9bc8-42cf-84da-2765f563e9b0"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("712d9ec3-5d2b-4b13-824f-71f00191dcca"),
                            Action = new Guid("e0759a42-47f0-4763-a26a-d5aa665bec35"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"),
                            Action = new Guid("0d68b142-e20a-446e-a832-0d6b0b65a164"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("6f05c382-8bca-4469-9424-c807a98c40d7"),
                            Object = "",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|1e04460243b54d7982f3fd6208a11960",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6743007c6f954d208c88a8601ce5e76d",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|e67be73df9ae4ce18fec1880cb518cb4",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|ea942538e68e49079394035336ee0ba8",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|32d24cb57ece46069c9419216ba42086",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|bf88953e3c434850a3fbb1e43ad53a3e",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|2a9230378b2d487b9a225ac0918acf3f",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|f4d98afdd336433287783c6945c81ea0",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|28b10049dd204f54b986873bc14ccfc7",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|3cfd481b46f24a4ab55cb8c0c9def02c",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6a598c7491ae437da5f4ad339bd11bb2",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|742cf945cbbc4a5782d61600a12cf8ca",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|853b6eb973ee438d9b098ffeedf36234",
                            AceType = 0
                        },
                        new
                        {
                            Tenant = -1,
                            Subject = new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"),
                            Action = new Guid("77777777-32ae-425f-99b5-83176061d1ae"),
                            Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|46cfa73af32046cf8d5bcd82e1d67f26",
                            AceType = 0
                        });
                });

            modelBuilder.Entity("ASC.Core.Common.EF.DbButton", b =>
                {
                    b.Property<int>("TariffId")
                        .HasColumnName("tariff_id")
                        .HasColumnType("integer");

                    b.Property<string>("PartnerId")
                        .HasColumnName("partner_id")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("ButtonUrl")
                        .IsRequired()
                        .HasColumnName("button_url")
                        .HasColumnType("text");

                    b.HasKey("TariffId", "PartnerId")
                        .HasName("tenants_buttons_pkey");

                    b.ToTable("tenants_buttons","onlyoffice");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.DbQuota", b =>
                {
                    b.Property<int>("Tenant")
                        .HasColumnName("tenant")
                        .HasColumnType("integer");

                    b.Property<int>("ActiveUsers")
                        .HasColumnName("active_users")
                        .HasColumnType("integer");

                    b.Property<string>("AvangateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("avangate_id")
                        .HasColumnType("character varying(128)")
                        .HasDefaultValueSql("NULL")
                        .HasMaxLength(128);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("character varying");

                    b.Property<string>("Features")
                        .HasColumnName("features")
                        .HasColumnType("text");

                    b.Property<long>("MaxFileSize")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("max_file_size")
                        .HasColumnType("bigint")
                        .HasDefaultValueSql("'0'");

                    b.Property<long>("MaxTotalSize")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("max_total_size")
                        .HasColumnType("bigint")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("character varying");

                    b.Property<decimal>("Price")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("price")
                        .HasColumnType("numeric(10,2)")
                        .HasDefaultValueSql("0.00");

                    b.Property<decimal>("Price2")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("price2")
                        .HasColumnType("numeric(10,2)")
                        .HasDefaultValueSql("0.00");

                    b.Property<bool>("Visible")
                        .HasColumnName("visible")
                        .HasColumnType("boolean");

                    b.HasKey("Tenant")
                        .HasName("tenants_quota_pkey");

                    b.ToTable("tenants_quota","onlyoffice");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.DbQuotaRow", b =>
                {
                    b.Property<int>("Tenant")
                        .HasColumnName("tenant")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .HasColumnName("path")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<long>("Counter")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("counter")
                        .HasColumnType("bigint")
                        .HasDefaultValueSql("'0'");

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("last_modified")
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Tag")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("tag")
                        .HasColumnType("character varying(1024)")
                        .HasDefaultValueSql("'0'")
                        .HasMaxLength(1024);

                    b.HasKey("Tenant", "Path")
                        .HasName("tenants_quotarow_pkey");

                    b.HasIndex("LastModified")
                        .HasName("last_modified_tenants_quotarow");

                    b.ToTable("tenants_quotarow","onlyoffice");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.DbTariff", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comment")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("comment")
                        .HasColumnType("character varying(255)")
                        .HasDefaultValueSql("NULL")
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreateOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("create_on")
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("Stamp")
                        .HasColumnName("stamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Tariff")
                        .HasColumnName("tariff")
                        .HasColumnType("integer");

                    b.Property<string>("TariffKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("tariff_key")
                        .HasColumnType("character varying(64)")
                        .HasDefaultValueSql("NULL")
                        .HasMaxLength(64);

                    b.Property<int>("Tenant")
                        .HasColumnName("tenant")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Tenant")
                        .HasName("tenant_tenants_tariff");

                    b.ToTable("tenants_tariff","onlyoffice");
                });
#pragma warning restore 612, 618
        }
    }
}
