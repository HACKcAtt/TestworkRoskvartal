using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestWork.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clinical_department",
                columns: table => new
                {
                    clinical_department_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    clinical_department_name = table.Column<string>(maxLength: 45, nullable: false),
                    clinical_department_existed_flag = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinical_department", x => x.clinical_department_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    roles_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    roles_name = table.Column<string>(maxLength: 45, nullable: false),
                    roles_existed_flag = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.roles_id);
                });

            migrationBuilder.CreateTable(
                name: "doctors",
                columns: table => new
                {
                    doctors_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    doctors_name = table.Column<string>(maxLength: 100, nullable: false),
                    doctors_specialization = table.Column<string>(maxLength: 45, nullable: false),
                    doctors_phone_number = table.Column<string>(maxLength: 20, nullable: false),
                    doctors_existed_flag = table.Column<bool>(nullable: false),
                    clinical_department_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctors", x => x.doctors_id);
                    table.ForeignKey(
                        name: "FK_doctors_clinical_department_clinical_department_id",
                        column: x => x.clinical_department_id,
                        principalTable: "clinical_department",
                        principalColumn: "clinical_department_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    users_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    users_name = table.Column<string>(maxLength: 100, nullable: false),
                    users_email = table.Column<string>(maxLength: 100, nullable: false),
                    users_phone_number = table.Column<string>(maxLength: 20, nullable: false),
                    users_address = table.Column<string>(maxLength: 100, nullable: false),
                    users_password = table.Column<string>(maxLength: 32, nullable: false),
                    users_birthday = table.Column<string>(maxLength: 10, nullable: false),
                    users_existed_flag = table.Column<bool>(nullable: false),
                    roles_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.users_id);
                    table.ForeignKey(
                        name: "FK_users_roles_roles_id",
                        column: x => x.roles_id,
                        principalTable: "roles",
                        principalColumn: "roles_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    customer_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    customer_notes = table.Column<string>(nullable: true),
                    customer_existed_flag = table.Column<bool>(nullable: false),
                    users_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.customer_id);
                    table.ForeignKey(
                        name: "FK_customers_users_users_id",
                        column: x => x.users_id,
                        principalTable: "users",
                        principalColumn: "users_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customers_illnesses",
                columns: table => new
                {
                    customers_illnesses_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    customers_illnesses_name = table.Column<string>(maxLength: 100, nullable: false),
                    customers_illnesses_description = table.Column<string>(nullable: true),
                    customers_illnesses_datetime_of_addition = table.Column<string>(nullable: true),
                    customers_illnesses_existed_flag = table.Column<bool>(nullable: false),
                    customer_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers_illnesses", x => x.customers_illnesses_id);
                    table.ForeignKey(
                        name: "FK_customers_illnesses_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "doctors_appointments",
                columns: table => new
                {
                    doctors_appointments_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    doctors_appointments_date = table.Column<string>(nullable: false),
                    doctors_appointments_time = table.Column<string>(nullable: false),
                    doctors_appointments_existed_flag = table.Column<bool>(nullable: false),
                    doctors_id = table.Column<int>(nullable: false),
                    customer_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctors_appointments", x => x.doctors_appointments_id);
                    table.ForeignKey(
                        name: "FK_doctors_appointments_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_doctors_appointments_doctors_doctors_id",
                        column: x => x.doctors_id,
                        principalTable: "doctors",
                        principalColumn: "doctors_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_users_id",
                table: "customers",
                column: "users_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_illnesses_customer_id",
                table: "customers_illnesses",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_doctors_clinical_department_id",
                table: "doctors",
                column: "clinical_department_id");

            migrationBuilder.CreateIndex(
                name: "IX_doctors_appointments_customer_id",
                table: "doctors_appointments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_doctors_appointments_doctors_id",
                table: "doctors_appointments",
                column: "doctors_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_roles_id",
                table: "users",
                column: "roles_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers_illnesses");

            migrationBuilder.DropTable(
                name: "doctors_appointments");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "doctors");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "clinical_department");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
