﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stressless_Service.Database_EFCore;

#nullable disable

namespace Stressless_Service.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("Stressless_Service.Models.AuthorizeModel", b =>
                {
                    b.Property<int>("AuthorizeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClientID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("TEXT");

                    b.Property<string>("LatestLogin")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MACAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AuthorizeID");

                    b.ToTable("Authorize");
                });

            modelBuilder.Entity("Stressless_Service.Models.CalenderEvents", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Event")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("Runtime")
                        .HasColumnType("TEXT");

                    b.HasKey("EventID");
                    b.ToTable("CalenderEvents");
                });

            modelBuilder.Entity("Stressless_Service.Models.CalenderModel", b =>
                {
                    b.Property<int>("CalenderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ConfigurationID")
                        .HasColumnType("INTEGER");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("EventDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("CalenderID");
                    b.ToTable("Calender");
                });

            modelBuilder.Entity("Stressless_Service.Models.ConfigurationModel", b =>
                {
                    b.Property<int>("ConfigurationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CalenderImport")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("DayEndTime")
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("DayStartTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WorkingDays")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ConfigurationID");
                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("Stressless_Service.Models.PromptModel", b =>
                {
                    b.Property<int>("PromptID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PromptID");
                    b.ToTable("Prompts");
                });

            modelBuilder.Entity("Stressless_Service.Models.ReminderModel", b =>
                {
                    b.Property<int>("ReminderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("ReminderID");
                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("Stressless_Service.Models.UsedPromptsModel", b =>
                {
                    b.Property<int>("UsedPromptID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastUsed")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PromptID")
                        .HasColumnType("INTEGER");

                    b.HasKey("UsedPromptID");
                    b.ToTable("UsedPrompts");
                });

            modelBuilder.Entity("Stressless_Service.Models.ConfigurationModel", b =>
                {
                    b.HasOne("Stressless_Service.Models.CalenderModel", null)
                        .WithOne("Configuration")
                        .HasForeignKey("ConfigurationID");
                });

            modelBuilder.Entity("Stressless_Service.Models.CalenderModel", b =>
                {
                    b.Navigation("Configuration");
                });
#pragma warning restore 612, 618
        }
    }
}
