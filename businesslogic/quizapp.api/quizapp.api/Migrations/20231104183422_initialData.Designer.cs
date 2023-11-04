﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using quizapp.api.Data;

#nullable disable

namespace quizapp.api.Migrations
{
    [DbContext(typeof(QuizDbContext))]
    [Migration("20231104183422_initialData")]
    partial class initialData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("quizapp.api.Models.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("quizapp.api.Models.Finding", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AnswerId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AnswerId")
                        .IsUnique();

                    b.ToTable("Findings");
                });

            modelBuilder.Entity("quizapp.api.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("QuestionNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionSeverityId")
                        .HasColumnType("int");

                    b.Property<int>("QuestionTypeId")
                        .HasColumnType("int");

                    b.Property<int>("SectionId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ZoneId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionSeverityId");

                    b.HasIndex("QuestionTypeId");

                    b.HasIndex("SectionId");

                    b.HasIndex("ZoneId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("quizapp.api.Models.QuestionSeverity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Severity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("QuestionSeverity");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Severity = 1
                        },
                        new
                        {
                            Id = 2,
                            Severity = 2
                        },
                        new
                        {
                            Id = 3,
                            Severity = 3
                        });
                });

            modelBuilder.Entity("quizapp.api.Models.QuestionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("QuestionTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            TypeName = "blank"
                        },
                        new
                        {
                            Id = 2,
                            TypeName = "singleSelection"
                        },
                        new
                        {
                            Id = 3,
                            TypeName = "multipleChoiceSelection"
                        });
                });

            modelBuilder.Entity("quizapp.api.Models.Recommendation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AnswerId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AnswerId");

                    b.ToTable("Recommendations");
                });

            modelBuilder.Entity("quizapp.api.Models.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sections");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "A"
                        },
                        new
                        {
                            Id = 2,
                            Name = "B"
                        },
                        new
                        {
                            Id = 3,
                            Name = "C"
                        },
                        new
                        {
                            Id = 4,
                            Name = "D"
                        },
                        new
                        {
                            Id = 5,
                            Name = "E"
                        },
                        new
                        {
                            Id = 6,
                            Name = "F"
                        },
                        new
                        {
                            Id = 7,
                            Name = "G"
                        },
                        new
                        {
                            Id = 8,
                            Name = "H"
                        },
                        new
                        {
                            Id = 9,
                            Name = "I"
                        },
                        new
                        {
                            Id = 10,
                            Name = "J"
                        },
                        new
                        {
                            Id = 11,
                            Name = "K"
                        },
                        new
                        {
                            Id = 12,
                            Name = "L"
                        },
                        new
                        {
                            Id = 13,
                            Name = "M"
                        },
                        new
                        {
                            Id = 14,
                            Name = "N"
                        },
                        new
                        {
                            Id = 15,
                            Name = "O"
                        },
                        new
                        {
                            Id = 16,
                            Name = "P"
                        },
                        new
                        {
                            Id = 17,
                            Name = "Q"
                        },
                        new
                        {
                            Id = 18,
                            Name = "R"
                        },
                        new
                        {
                            Id = 19,
                            Name = "S"
                        },
                        new
                        {
                            Id = 20,
                            Name = "T"
                        },
                        new
                        {
                            Id = 21,
                            Name = "U"
                        },
                        new
                        {
                            Id = 22,
                            Name = "V"
                        },
                        new
                        {
                            Id = 23,
                            Name = "W"
                        },
                        new
                        {
                            Id = 24,
                            Name = "X"
                        },
                        new
                        {
                            Id = 25,
                            Name = "Y"
                        },
                        new
                        {
                            Id = 26,
                            Name = "Z"
                        });
                });

            modelBuilder.Entity("quizapp.api.Models.Zone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Zones");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "PARKING"
                        },
                        new
                        {
                            Id = 2,
                            Name = "PATHWAYS"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ACCESSIBLE_ENTERANCE"
                        },
                        new
                        {
                            Id = 4,
                            Name = "INTERIOR_ROUTES"
                        },
                        new
                        {
                            Id = 5,
                            Name = "VOTING_AREAS"
                        });
                });

            modelBuilder.Entity("quizapp.api.Models.Answer", b =>
                {
                    b.HasOne("quizapp.api.Models.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("quizapp.api.Models.Finding", b =>
                {
                    b.HasOne("quizapp.api.Models.Answer", "Answer")
                        .WithOne("Findings")
                        .HasForeignKey("quizapp.api.Models.Finding", "AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Answer");
                });

            modelBuilder.Entity("quizapp.api.Models.Question", b =>
                {
                    b.HasOne("quizapp.api.Models.QuestionSeverity", "QuestionSeverity")
                        .WithMany()
                        .HasForeignKey("QuestionSeverityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("quizapp.api.Models.QuestionType", "QuestionType")
                        .WithMany("Questions")
                        .HasForeignKey("QuestionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("quizapp.api.Models.Section", "Section")
                        .WithMany()
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("quizapp.api.Models.Zone", "Zone")
                        .WithMany()
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuestionSeverity");

                    b.Navigation("QuestionType");

                    b.Navigation("Section");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("quizapp.api.Models.Recommendation", b =>
                {
                    b.HasOne("quizapp.api.Models.Answer", "Answer")
                        .WithMany("Recommendations")
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Answer");
                });

            modelBuilder.Entity("quizapp.api.Models.Answer", b =>
                {
                    b.Navigation("Findings")
                        .IsRequired();

                    b.Navigation("Recommendations");
                });

            modelBuilder.Entity("quizapp.api.Models.Question", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("quizapp.api.Models.QuestionType", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}