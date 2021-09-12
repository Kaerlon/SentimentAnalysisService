﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SentimentAnalysis.Bot.Data;

namespace SentimentAnalysis.Bot.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20210912154222_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SentimentAnalysis.Bot.Models.ChatSettings", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("ServantListNotifications")
                        .HasColumnType("bit");

                    b.Property<bool>("SupportListNotifications")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("ChatSettings");
                });

            modelBuilder.Entity("SentimentAnalysis.Bot.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("People");
                });

            modelBuilder.Entity("SentimentAnalysis.Bot.Models.RegisteredChat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ChatId1")
                        .HasColumnType("bigint");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("ChatId1");

                    b.HasIndex("PersonId");

                    b.ToTable("RegisteredChats");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.Data", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "Key");

                    b.ToTable("Data");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.Newsletter", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Key");

                    b.ToTable("Newsletters");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.NewsletterChat", b =>
                {
                    b.Property<string>("NewsletterKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.HasKey("NewsletterKey", "ChatId");

                    b.HasIndex("ChatId");

                    b.ToTable("NewsletterChats");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.TelegramChat", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool?>("CanSetStickerSet")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InviteLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StickerSetName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SentimentAnalysis.Bot.Models.ChatSettings", b =>
                {
                    b.HasOne("Telegram.Bot.Advanced.DbContexts.TelegramChat", "TelegramChat")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TelegramChat");
                });

            modelBuilder.Entity("SentimentAnalysis.Bot.Models.Person", b =>
                {
                    b.HasOne("Telegram.Bot.Advanced.DbContexts.TelegramChat", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SentimentAnalysis.Bot.Models.RegisteredChat", b =>
                {
                    b.HasOne("Telegram.Bot.Advanced.DbContexts.TelegramChat", null)
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Telegram.Bot.Advanced.DbContexts.TelegramChat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId1");

                    b.HasOne("SentimentAnalysis.Bot.Models.Person", "Person")
                        .WithMany("RegisteredChats")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.Data", b =>
                {
                    b.HasOne("Telegram.Bot.Advanced.DbContexts.TelegramChat", "Chat")
                        .WithMany("Data")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.NewsletterChat", b =>
                {
                    b.HasOne("Telegram.Bot.Advanced.DbContexts.TelegramChat", "Chat")
                        .WithMany("NewsletterChats")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Telegram.Bot.Advanced.DbContexts.Newsletter", "Newsletter")
                        .WithMany("NewsletterChats")
                        .HasForeignKey("NewsletterKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Newsletter");
                });

            modelBuilder.Entity("SentimentAnalysis.Bot.Models.Person", b =>
                {
                    b.Navigation("RegisteredChats");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.Newsletter", b =>
                {
                    b.Navigation("NewsletterChats");
                });

            modelBuilder.Entity("Telegram.Bot.Advanced.DbContexts.TelegramChat", b =>
                {
                    b.Navigation("Data");

                    b.Navigation("NewsletterChats");
                });
#pragma warning restore 612, 618
        }
    }
}
