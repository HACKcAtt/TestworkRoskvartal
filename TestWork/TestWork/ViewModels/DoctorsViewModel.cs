using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWork.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.ViewModels
{
    public class DoctorsViewModel : IValidatableObject
    {
        public virtual Doctors Doctor { get; set; }
        public virtual Customer Customers { get; set; }
        public virtual List<ClinicalDepartment> ClinicalDepartments { get; set; }
        public virtual List<DoctorsAppointments> DoctorsAppointmentss { get; set; }
        public virtual DoctorsAppointments DoctorsAppointment { get; set; }
        public string doctorsOperationFlag { get; set; }
        [Required(ErrorMessage = "Требуется дата начала периода работы доктора.")]
        [DataType(DataType.Date, ErrorMessage = "Поле содержит недопустимые символы.")]
        public string doctorsStartDate { get; set; }
        [Required(ErrorMessage = "Требуется дата окончания периода работы доктора.")]
        [DataType(DataType.Date, ErrorMessage = "Поле содержит недопустимые символы.")]
        public string doctorsStopDate { get; set; }
        //public int customerId { get; set; }
        //[DataType(DataType.Date, ErrorMessage = "Поле содержит недопустимые символы.")]
        //public string customerAppointmentDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var property = new[] { "doctorsStopDate" };
            if (Convert.ToDateTime(doctorsStopDate) < Convert.ToDateTime(doctorsStartDate))
            {
                yield return new ValidationResult("Дата окончания периода работы доктора ну никак не может быть раньше даты начала периода работы доктора. Если только доктор не научился путешествовать во времени.", property);
            }
        }
    }
}
