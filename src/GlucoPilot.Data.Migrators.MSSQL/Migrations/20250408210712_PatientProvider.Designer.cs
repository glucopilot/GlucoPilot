﻿// <auto-generated />
using System;
using GlucoPilot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GlucoPilot.Data.Migrators.MSSQL.Migrations
{
    [DbContext(typeof(GlucoPilotDbContext))]
    [Migration("20250408210712_PatientProvider")]
    partial class PatientProvider
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GlucoPilot.Data.Entities.Ingredient", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Calories")
                        .HasColumnType("int");

                    b.Property<int>("Carbs")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Fat")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Protein")
                        .HasColumnType("int");

                    b.Property<int>("Uom")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ingredients");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Injection", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("InsulinId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Units")
                        .HasColumnType("float");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("InsulinId");

                    b.HasIndex("UserId");

                    b.ToTable("injections");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Insulin", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<double?>("Duration")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("PeakTime")
                        .HasColumnType("float");

                    b.Property<double?>("Scale")
                        .HasColumnType("float");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("insulin");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Meal", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("meals");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.MealIngredient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MealId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.HasIndex("MealId");

                    b.ToTable("meals_ingredients");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Reading", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<double>("GlucoseLevel")
                        .HasColumnType("float");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("readings");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Treatment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("InjectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MealId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ReadingId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("InjectionId");

                    b.HasIndex("MealId");

                    b.HasIndex("ReadingId");

                    b.HasIndex("UserId");

                    b.ToTable("treatments");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AcceptedTerms")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("nvarchar(320)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("Updated")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("users");

                    b.HasDiscriminator().HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Patient", b =>
                {
                    b.HasBaseType("GlucoPilot.Data.Entities.User");

                    b.Property<int>("GlucoseProvider")
                        .HasColumnType("int");

                    b.Property<string>("PatientId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("UserId");

                    b.ToTable("users");

                    b.HasDiscriminator().HasValue("Patient");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Ingredient", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Injection", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.Insulin", "Insulin")
                        .WithMany()
                        .HasForeignKey("InsulinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GlucoPilot.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Insulin");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Insulin", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Meal", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.MealIngredient", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.Ingredient", "Ingredient")
                        .WithMany("Meals")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GlucoPilot.Data.Entities.Meal", "Meal")
                        .WithMany("MealIngredients")
                        .HasForeignKey("MealId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Ingredient");

                    b.Navigation("Meal");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Reading", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Treatment", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.Injection", "Injection")
                        .WithMany()
                        .HasForeignKey("InjectionId");

                    b.HasOne("GlucoPilot.Data.Entities.Meal", "Meal")
                        .WithMany()
                        .HasForeignKey("MealId");

                    b.HasOne("GlucoPilot.Data.Entities.Reading", "Reading")
                        .WithMany()
                        .HasForeignKey("ReadingId");

                    b.HasOne("GlucoPilot.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Injection");

                    b.Navigation("Meal");

                    b.Navigation("Reading");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Patient", b =>
                {
                    b.HasOne("GlucoPilot.Data.Entities.User", null)
                        .WithMany("Patients")
                        .HasForeignKey("UserId");

                    b.OwnsOne("GlucoPilot.Data.Entities.AuthTicket", "AuthTicket", b1 =>
                        {
                            b1.Property<Guid>("PatientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<long>("Duration")
                                .HasColumnType("bigint");

                            b1.Property<long>("Expires")
                                .HasColumnType("bigint");

                            b1.Property<string>("Token")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("PatientId");

                            b1.ToTable("users");

                            b1.WithOwner()
                                .HasForeignKey("PatientId");
                        });

                    b.Navigation("AuthTicket");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Ingredient", b =>
                {
                    b.Navigation("Meals");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.Meal", b =>
                {
                    b.Navigation("MealIngredients");
                });

            modelBuilder.Entity("GlucoPilot.Data.Entities.User", b =>
                {
                    b.Navigation("Patients");
                });
#pragma warning restore 612, 618
        }
    }
}
