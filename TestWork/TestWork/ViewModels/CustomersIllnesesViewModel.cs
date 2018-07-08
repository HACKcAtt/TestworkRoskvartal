using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWork.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.ViewModels
{
    public class CustomersIllnesesViewModel
    {
        public virtual Customer Customers { get; set; }
        public virtual CustomersIllnesses CustomersIllness { get; set; }
        public int customerIdd { get; set; }
        public string customersOperationFlag { get; set; }
    }
}
