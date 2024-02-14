﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlcConnectionService.DATA;

#nullable disable

namespace PlcConnectionService.DATA.DataAccess.Migrations
{
    [DbContext(typeof(BaseDbContext))]
    [Migration("20240214124437_Updateinitial")]
    partial class Updateinitial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PlcConnectionService.Entities.Entities.PlcData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AdimNo")
                        .HasColumnType("int");

                    b.Property<long>("Alinacak")
                        .HasColumnType("bigint");

                    b.Property<long>("Alinan")
                        .HasColumnType("bigint");

                    b.Property<int>("BatchNo")
                        .HasColumnType("int");

                    b.Property<int>("Counter")
                        .HasColumnType("int");

                    b.Property<long>("HammaddeID")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("KayitTarihi")
                        .HasColumnType("datetime2");

                    b.Property<long>("PartiID")
                        .HasColumnType("bigint");

                    b.Property<long>("ReceteID")
                        .HasColumnType("bigint");

                    b.Property<long>("Shut")
                        .HasColumnType("bigint");

                    b.Property<int>("SiloNo")
                        .HasColumnType("int");

                    b.Property<int>("TN")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PlcDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
