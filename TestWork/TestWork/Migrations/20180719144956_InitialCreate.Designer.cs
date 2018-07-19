﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestWork.Models;

namespace TestWork.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20180719144956_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TestWork.Models.ClinicalDepartment", b =>
                {
                    b.Property<int>("ClinicalDepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("clinical_department_id");

                    b.Property<bool>("ClinicalDepartmentExistedFlag")
                        .HasColumnName("clinical_department_existed_flag");

                    b.Property<string>("ClinicalDepartmentName")
                        .IsRequired()
                        .HasColumnName("clinical_department_name")
                        .HasMaxLength(45);

                    b.HasKey("ClinicalDepartmentId");

                    b.ToTable("clinical_department");
                });

            modelBuilder.Entity("TestWork.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("customer_id");

                    b.Property<bool>("CustomerExistedFlag")
                        .HasColumnName("customer_existed_flag");

                    b.Property<string>("CustomerNotes")
                        .HasColumnName("customer_notes");

                    b.Property<int>("UsersId")
                        .HasColumnName("users_id");

                    b.HasKey("CustomerId");

                    b.HasIndex("UsersId")
                        .IsUnique();

                    b.ToTable("customers");
                });

            modelBuilder.Entity("TestWork.Models.CustomersIllnesses", b =>
                {
                    b.Property<int>("CustomersIllnessesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("customers_illnesses_id");

                    b.Property<int>("CustomerId")
                        .HasColumnName("customer_id");

                    b.Property<string>("CustomersIllnessesDateTimeOfAddition")
                        .HasColumnName("customers_illnesses_datetime_of_addition");

                    b.Property<string>("CustomersIllnessesDescription")
                        .HasColumnName("customers_illnesses_description");

                    b.Property<string>("CustomersIllnessesName")
                        .IsRequired()
                        .HasColumnName("customers_illnesses_name")
                        .HasMaxLength(100);

                    b.Property<bool>("CustomersIllnessesrExistedFlag")
                        .HasColumnName("customers_illnesses_existed_flag");

                    b.HasKey("CustomersIllnessesId");

                    b.HasIndex("CustomerId");

                    b.ToTable("customers_illnesses");
                });

            modelBuilder.Entity("TestWork.Models.Doctors", b =>
                {
                    b.Property<int>("DoctorsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("doctors_id");

                    b.Property<int>("ClinicalDepartmentId")
                        .HasColumnName("clinical_department_id");

                    b.Property<bool>("DoctorsExistedFlag")
                        .HasColumnName("doctors_existed_flag");

                    b.Property<string>("DoctorsName")
                        .IsRequired()
                        .HasColumnName("doctors_name")
                        .HasMaxLength(100);

                    b.Property<string>("DoctorsPhoneNumber")
                        .IsRequired()
                        .HasColumnName("doctors_phone_number")
                        .HasMaxLength(20);

                    b.Property<string>("DoctorsSpecialization")
                        .IsRequired()
                        .HasColumnName("doctors_specialization")
                        .HasMaxLength(45);

                    b.HasKey("DoctorsId");

                    b.HasIndex("ClinicalDepartmentId");

                    b.ToTable("doctors");
                });

            modelBuilder.Entity("TestWork.Models.DoctorsAppointments", b =>
                {
                    b.Property<int>("DoctorAppointmentsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("doctors_appointments_id");

                    b.Property<int?>("CustomerId")
                        .HasColumnName("customer_id");

                    b.Property<string>("DoctorAppointmentsDate")
                        .IsRequired()
                        .HasColumnName("doctors_appointments_date");

                    b.Property<bool>("DoctorAppointmentsExistedFlag")
                        .HasColumnName("doctors_appointments_existed_flag");

                    b.Property<string>("DoctorAppointmentsTime")
                        .IsRequired()
                        .HasColumnName("doctors_appointments_time");

                    b.Property<int>("DoctorsId")
                        .HasColumnName("doctors_id");

                    b.HasKey("DoctorAppointmentsId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("DoctorsId");

                    b.ToTable("doctors_appointments");
                });

            modelBuilder.Entity("TestWork.Models.Roles", b =>
                {
                    b.Property<int>("RolesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("roles_id");

                    b.Property<bool>("RolesExistedFlag")
                        .HasColumnName("roles_existed_flag");

                    b.Property<string>("RolesName")
                        .IsRequired()
                        .HasColumnName("roles_name")
                        .HasMaxLength(45);

                    b.HasKey("RolesId");

                    b.ToTable("roles");
                });

            modelBuilder.Entity("TestWork.Models.Users", b =>
                {
                    b.Property<int>("UsersId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("users_id");

                    b.Property<int>("RolesId")
                        .HasColumnName("roles_id");

                    b.Property<string>("UsersAddress")
                        .IsRequired()
                        .HasColumnName("users_address")
                        .HasMaxLength(100);

                    b.Property<string>("UsersBirthday")
                        .IsRequired()
                        .HasColumnName("users_birthday")
                        .HasMaxLength(10);

                    b.Property<string>("UsersEmail")
                        .IsRequired()
                        .HasColumnName("users_email")
                        .HasMaxLength(100);

                    b.Property<bool>("UsersExistedFlag")
                        .HasColumnName("users_existed_flag");

                    b.Property<string>("UsersName")
                        .IsRequired()
                        .HasColumnName("users_name")
                        .HasMaxLength(100);

                    b.Property<string>("UsersPassword")
                        .IsRequired()
                        .HasColumnName("users_password")
                        .HasMaxLength(32);

                    b.Property<string>("UsersPhoneNumber")
                        .IsRequired()
                        .HasColumnName("users_phone_number")
                        .HasMaxLength(20);

                    b.HasKey("UsersId");

                    b.HasIndex("RolesId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("TestWork.Models.Customer", b =>
                {
                    b.HasOne("TestWork.Models.Users", "Users")
                        .WithOne("Customer")
                        .HasForeignKey("TestWork.Models.Customer", "UsersId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TestWork.Models.CustomersIllnesses", b =>
                {
                    b.HasOne("TestWork.Models.Customer", "Customer")
                        .WithMany("CustomersIllnesses")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TestWork.Models.Doctors", b =>
                {
                    b.HasOne("TestWork.Models.ClinicalDepartment", "ClinicalDepartment")
                        .WithMany("Doctors")
                        .HasForeignKey("ClinicalDepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TestWork.Models.DoctorsAppointments", b =>
                {
                    b.HasOne("TestWork.Models.Customer", "Customer")
                        .WithMany("DoctorsAppointments")
                        .HasForeignKey("CustomerId");

                    b.HasOne("TestWork.Models.Doctors", "Doctors")
                        .WithMany("DoctorsAppointments")
                        .HasForeignKey("DoctorsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TestWork.Models.Users", b =>
                {
                    b.HasOne("TestWork.Models.Roles", "UserRoles")
                        .WithMany("Users")
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}