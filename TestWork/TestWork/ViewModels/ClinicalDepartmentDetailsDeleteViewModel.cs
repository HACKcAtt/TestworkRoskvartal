using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWork.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.ViewModels
{
    public class ClinicalDepartmentDetailsDeleteViewModel
    {
        public virtual ClinicalDepartment ClinicalDepartment { get; set; }
        public string clinicalDepartmentOperationFlag { get; set; }
    }
}
