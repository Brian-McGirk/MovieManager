﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MovieManager.Data;
using System;

namespace MovieManager.Migrations
{
    [DbContext(typeof(MovieDbContext))]
    [Migration("20180628001224_MediaMigration")]
    partial class MediaMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MovieManager.Models.Media", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AirDate");

                    b.Property<string>("EpisodeName");

                    b.Property<int>("EpisodeNumber");

                    b.Property<int>("Season");

                    b.Property<bool>("Seen");

                    b.Property<string>("TvShow");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Media");
                });

            modelBuilder.Entity("MovieManager.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Password");

                    b.Property<string>("UserName");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MovieManager.Models.Media", b =>
                {
                    b.HasOne("MovieManager.Models.User", "User")
                        .WithMany("Medias")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}