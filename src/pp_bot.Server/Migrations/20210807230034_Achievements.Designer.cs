﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using pp_bot.Server.Models;

namespace pp_bot.Server.Migrations
{
    [DbContext(typeof(PP_Context))]
    [Migration("20210807230034_Achievements")]
    partial class Achievements
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("AchievementBotUserChat", b =>
                {
                    b.Property<int>("AcquiredAchievementsId")
                        .HasColumnType("integer");

                    b.Property<int>("UsersAcquiredChatUsersId")
                        .HasColumnType("integer");

                    b.Property<long>("UsersAcquiredUserChatsChatId")
                        .HasColumnType("bigint");

                    b.HasKey("AcquiredAchievementsId", "UsersAcquiredChatUsersId", "UsersAcquiredUserChatsChatId");

                    b.HasIndex("UsersAcquiredChatUsersId", "UsersAcquiredUserChatsChatId");

                    b.ToTable("AchievementBotUserChat");
                });

            modelBuilder.Entity("pp_bot.Server.Models.Achievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ImageFileName")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("pp_bot.Server.Models.BotUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BotUsers");
                });

            modelBuilder.Entity("pp_bot.Server.Models.BotUserChat", b =>
                {
                    b.Property<int>("ChatUsersId")
                        .HasColumnType("integer");

                    b.Property<long>("UserChatsChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("LastManipulationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PPLength")
                        .HasColumnType("integer");

                    b.HasKey("ChatUsersId", "UserChatsChatId");

                    b.HasIndex("UserChatsChatId");

                    b.ToTable("BotUserChat");
                });

            modelBuilder.Entity("pp_bot.Server.Models.Chat", b =>
                {
                    b.Property<long>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("ChatId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("AchievementBotUserChat", b =>
                {
                    b.HasOne("pp_bot.Server.Models.Achievement", null)
                        .WithMany()
                        .HasForeignKey("AcquiredAchievementsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pp_bot.Server.Models.BotUserChat", null)
                        .WithMany()
                        .HasForeignKey("UsersAcquiredChatUsersId", "UsersAcquiredUserChatsChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("pp_bot.Server.Models.BotUserChat", b =>
                {
                    b.HasOne("pp_bot.Server.Models.BotUser", "User")
                        .WithMany("UserChats")
                        .HasForeignKey("ChatUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pp_bot.Server.Models.Chat", "Chat")
                        .WithMany("ChatUsers")
                        .HasForeignKey("UserChatsChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("pp_bot.Server.Models.BotUser", b =>
                {
                    b.Navigation("UserChats");
                });

            modelBuilder.Entity("pp_bot.Server.Models.Chat", b =>
                {
                    b.Navigation("ChatUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
