﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RimionshipServer.Data;

#nullable disable

namespace RimionshipServer.Migrations
{
    [DbContext(typeof(RimionDbContext))]
    partial class RimionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("RimionshipServer.Data.AllowedMod", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PackageId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("SteamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PackageId")
                        .IsUnique();

                    b.HasIndex("SteamId")
                        .IsUnique();

                    b.ToTable("AllowedMods");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.BaseEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Sqlite", "Autoincrement");

                    b.Property<long>("TimeTicks")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UId");

                    b.ToTable("BaseEntry");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Caravans", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Caravans");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Colonists", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Colonists");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.ColonistsKilled", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("ColonistsKilled");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.ColonistsNeedTending", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("ColonistsNeedTending");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Conditions", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Conditions");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DamageDealt", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("HiddenId");

                    b.ToTable("DamageDealt");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DamageTakenPawns", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("HiddenId");

                    b.ToTable("DamageTakenPawns");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DamageTakenThings", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("HiddenId");

                    b.ToTable("DamageTakenThings");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DownedColonists", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("DownedColonists");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Electricity", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Electricity");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Enemies", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Enemies");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Fire", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Fire");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Food", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Food");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.GreatestPopulation", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("GreatestPopulation");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.InGameHours", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("InGameHours");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.MapCount", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("MapCount");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.MedicalConditions", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("MedicalConditions");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Medicine", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Medicine");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.MentalColonists", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("MentalColonists");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.NumRaidsEnemy", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("NumRaidsEnemy");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.NumThreatBigs", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("NumThreatBigs");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Prisoners", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Prisoners");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Rooms", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.TamedAnimals", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("TamedAnimals");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Temperature", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Temperature");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Visitors", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Visitors");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Wealth", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("Wealth");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.WeaponDps", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("WeaponDps");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.WildAnimals", b =>
                {
                    b.Property<int>("HiddenId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("HiddenId");

                    b.ToTable("WildAnimals");
                });

            modelBuilder.Entity("RimionshipServer.Data.RimionUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("RimionshipServer.Data.RimionUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("RimionshipServer.Data.RimionUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RimionshipServer.Data.RimionUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("RimionshipServer.Data.RimionUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Caravans", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Caravans", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Colonists", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Colonists", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.ColonistsKilled", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.ColonistsKilled", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.ColonistsNeedTending", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.ColonistsNeedTending", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Conditions", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Conditions", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DamageDealt", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.DamageDealt", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DamageTakenPawns", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.DamageTakenPawns", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DamageTakenThings", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.DamageTakenThings", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.DownedColonists", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.DownedColonists", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Electricity", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Electricity", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Enemies", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Enemies", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Fire", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Fire", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Food", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Food", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.GreatestPopulation", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.GreatestPopulation", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.InGameHours", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.InGameHours", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.MapCount", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.MapCount", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.MedicalConditions", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.MedicalConditions", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Medicine", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Medicine", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.MentalColonists", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.MentalColonists", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.NumRaidsEnemy", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.NumRaidsEnemy", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.NumThreatBigs", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.NumThreatBigs", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Prisoners", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Prisoners", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Rooms", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Rooms", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.TamedAnimals", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.TamedAnimals", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Temperature", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Temperature", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Visitors", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Visitors", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.Wealth", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.Wealth", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.WeaponDps", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.WeaponDps", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });

            modelBuilder.Entity("RimionshipServer.Data.Detailed.WildAnimals", b =>
                {
                    b.HasOne("RimionshipServer.Data.Detailed.BaseEntry", "Id")
                        .WithOne()
                        .HasForeignKey("RimionshipServer.Data.Detailed.WildAnimals", "HiddenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Id");
                });
#pragma warning restore 612, 618
        }
    }
}
