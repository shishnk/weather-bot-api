﻿// <auto-generated />
using System;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseApp.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240324163628_Initialize")]
    partial class Initialize
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DatabaseApp.Domain.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("ID");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
                    NpgsqlPropertyBuilderExtensions.HasIdentityOptions(b.Property<int>("Id"), 1L, null, null, null, null, null);

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("TIMESTAMP");

                    b.Property<int>("TelegramId")
                        .HasColumnType("integer")
                        .HasColumnName("TELEGRAM_ID");

                    b.HasKey("Id");

                    b.ToTable("USERS", (string)null);
                });

            modelBuilder.Entity("DatabaseApp.Domain.Models.UserWeatherSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("ID");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
                    NpgsqlPropertyBuilderExtensions.HasIdentityOptions(b.Property<int>("Id"), 1L, null, null, null, null, null);

                    b.Property<TimeSpan>("ResendInterval")
                        .HasColumnType("interval")
                        .HasColumnName("RESEND_INTERVAL");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("USER_ID");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("WEATHER_SUBSCRIPTIONS", (string)null);
                });

            modelBuilder.Entity("DatabaseApp.Domain.Models.User", b =>
                {
                    b.OwnsOne("DatabaseApp.Domain.Models.UserMetadata", "Metadata", b1 =>
                        {
                            b1.Property<int>("UserId")
                                .HasColumnType("integer");

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("MOBILE_NUMBER");

                            b1.Property<string>("Username")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("USERNAME");

                            b1.HasKey("UserId");

                            b1.ToTable("USERS");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Metadata")
                        .IsRequired();
                });

            modelBuilder.Entity("DatabaseApp.Domain.Models.UserWeatherSubscription", b =>
                {
                    b.HasOne("DatabaseApp.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_USER_ID");

                    b.OwnsOne("DatabaseApp.Domain.Models.Location", "Location", b1 =>
                        {
                            b1.Property<int>("UserWeatherSubscriptionId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("LOCATION");

                            b1.HasKey("UserWeatherSubscriptionId");

                            b1.ToTable("WEATHER_SUBSCRIPTIONS");

                            b1.WithOwner()
                                .HasForeignKey("UserWeatherSubscriptionId");
                        });

                    b.Navigation("Location")
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
