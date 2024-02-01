﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApiDataContext))]
    partial class ApiDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Entities.Url", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OriginalUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortenedUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Urls");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreatedAt = new DateTime(2024, 2, 1, 18, 45, 59, 51, DateTimeKind.Utc).AddTicks(2237),
                            OriginalUrl = "https://www.youtube.com/",
                            ShortenedUrl = ""
                        },
                        new
                        {
                            Id = 2L,
                            CreatedAt = new DateTime(2024, 2, 1, 18, 45, 59, 51, DateTimeKind.Utc).AddTicks(2239),
                            OriginalUrl = "https://www.youtube.com/",
                            ShortenedUrl = ""
                        },
                        new
                        {
                            Id = 3L,
                            CreatedAt = new DateTime(2024, 2, 1, 18, 45, 59, 51, DateTimeKind.Utc).AddTicks(2241),
                            OriginalUrl = "https://www.youtube.com/",
                            ShortenedUrl = ""
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
