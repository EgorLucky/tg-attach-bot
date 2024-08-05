﻿// <auto-generated />
using System;
using DomainLogic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LongPollingWorker.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DomainLogic.Entities.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("Height")
                        .HasColumnType("integer");

                    b.Property<string[]>("KeyWords")
                        .HasColumnType("text[]");

                    b.Property<DateTimeOffset?>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<long>("TelegramUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("ThumbFileId")
                        .HasColumnType("text");

                    b.Property<int?>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("DomainLogic.Entities.TelegramUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TelegramUsers");
                });

            modelBuilder.Entity("DomainLogic.Entities.File", b =>
                {
                    b.HasOne("DomainLogic.Entities.TelegramUser", null)
                        .WithMany()
                        .HasForeignKey("TelegramUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Telegram.Bot.Types.PhotoSize", "OtherPhotoSizes", b1 =>
                        {
                            b1.Property<Guid>("FileId1")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<string>("FileId")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<long?>("FileSize")
                                .HasColumnType("bigint");

                            b1.Property<string>("FileUniqueId")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<int>("Height")
                                .HasColumnType("integer");

                            b1.Property<int>("Width")
                                .HasColumnType("integer");

                            b1.HasKey("FileId1", "Id");

                            b1.ToTable("Files");

                            b1.ToJson("OtherPhotoSizes");

                            b1.WithOwner()
                                .HasForeignKey("FileId1");
                        });

                    b.Navigation("OtherPhotoSizes");
                });
#pragma warning restore 612, 618
        }
    }
}
