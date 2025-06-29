﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductProvider.Models.Data;

#nullable disable

namespace ProductProvider.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    [Migration("20250305130615_UpdateProductAndReservationEntities")]
    partial class UpdateProductAndReservationEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductProvider.Models.Data.Entities.ProductEntity", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BusinessType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CEO")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfEmployees")
                        .HasColumnType("int");

                    b.Property<string>("OrganizationNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReservedUntil")
                        .HasColumnType("datetime2");

                    b.Property<int>("Revenue")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SoldUntil")
                        .HasColumnType("datetime2");

                    b.HasKey("ProductId");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("ProductProvider.Models.Data.Entities.ReservationEntity", b =>
                {
                    b.Property<Guid>("ReservationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BusinessTypes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cities")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CitiesByRegion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("MaxNumberOfEmployees")
                        .HasColumnType("int");

                    b.Property<int?>("MaxRevenue")
                        .HasColumnType("int");

                    b.Property<int?>("MinNumberOfEmployees")
                        .HasColumnType("int");

                    b.Property<int?>("MinRevenue")
                        .HasColumnType("int");

                    b.Property<string>("PostalCodes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Regions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReservedFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("SoldFrom")
                        .HasColumnType("datetime2");

                    b.HasKey("ReservationId");

                    b.ToTable("Reservations", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
