using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWork.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.ViewModels
{
    public class DoctorsAppointmentsViewModel
    {
        public virtual List<DoctorsAppointments> DoctorsAppointments { get; set; }
        public virtual List<Doctors> Doctors { get; set; }
        public virtual List<Customer> Customers { get; set; }
        public string doctorsAppointmentsOperationFlag { get; set; }
    }
}
