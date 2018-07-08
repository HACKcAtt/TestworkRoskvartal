using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWork.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.ViewModels
{
    public class CustomersViewModel
    {
        public virtual Customer Customers { get; set; }
        public virtual List<Doctors> Doctors { get; set; }
        public string customersOperationFlag { get; set; }
    }
}
