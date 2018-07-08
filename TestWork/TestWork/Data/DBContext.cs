using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EFLogging;

namespace TestWork.Models
{
    public class DBContext : DbContext
    {
        public DBContext (DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var lf = new LoggerFactory();
        //    lf.AddProvider(new MyLoggerProvider());
        //    optionsBuilder.UseLoggerFactory(lf);
        //}

        public DbSet<TestWork.Models.Customer> Customer { get; set; }
        public DbSet<TestWork.Models.ClinicalDepartment> ClinicalDepartment { get; set; }
        public DbSet<TestWork.Models.CustomersIllnesses> CustomersIllnesses { get; set; }
        public DbSet<TestWork.Models.Doctors> Doctors { get; set; }
        public DbSet<TestWork.Models.DoctorsAppointments> DoctorsAppointments { get; set; }
        public DbSet<TestWork.Models.Roles> Roles { get; set; }
        public DbSet<TestWork.Models.Users> Users { get; set; }
    }
}
