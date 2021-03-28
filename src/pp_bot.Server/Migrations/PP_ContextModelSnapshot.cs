﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using pp_bot.Server.Models;

namespace pp_bot.Server.Migrations
{
    [DbContext(typeof(PP_Context))]
    partial class PP_ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("BotUserChat", b =>
                {
                    b.Property<int>("ChatUsersId")
                        .HasColumnType("integer");

                    b.Property<int>("UserChatsId")
                        .HasColumnType("integer");

                    b.HasKey("ChatUsersId", "UserChatsId");

                    b.HasIndex("UserChatsId");

                    b.ToTable("BotUserChat");
                });

            modelBuilder.Entity("pp_bot.Server.Models.BotUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("LastManipulationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("PPLength")
                        .HasColumnType("bigint");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BotUsers");
                });

            modelBuilder.Entity("pp_bot.Server.Models.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("ChatName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("BotUserChat", b =>
                {
                    b.HasOne("pp_bot.Server.Models.BotUser", null)
                        .WithMany()
                        .HasForeignKey("ChatUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pp_bot.Server.Models.Chat", null)
                        .WithMany()
                        .HasForeignKey("UserChatsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
