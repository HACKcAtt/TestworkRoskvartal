using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.Models
{
    [Table("doctors_appointments")]
    public class DoctorsAppointments
    {
        // ID приёма. Первичный ключ.
        [Key]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_appointments_id")]
        public int DoctorAppointmentsId { get; set; }

        // Дата приёма.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_appointments_date")]
        [Required(ErrorMessage = "Требуется дата приёма.")]
        [DataType(DataType.Date, ErrorMessage = "Поле содержит недопустимые символы.")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Дата приёма")]
        public string DoctorAppointmentsDate { get; set; }

        // Время приёма.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_appointments_time")]
        [Required(ErrorMessage = "Требуется время приёма.")]
        [DataType(DataType.Time, ErrorMessage = "Поле содержит недопустимые символы.")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Время приёма")]
        public string DoctorAppointmentsTime{ get; set; }

        // Флаг существования.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_appointments_existed_flag")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Флаг существования")]
        public bool DoctorAppointmentsExistedFlag { get; set; }

        // Id доктора.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_id")]
        public int DoctorsId { get; set; }

        // Внешний ключ на Id доктора.
        [ForeignKey("DoctorsId")]
        public Doctors Doctors { get; set; }

        // Id пациента.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("customer_id")]
        public int? CustomerId { get; set; }

        // Внешний ключ на Id пациента.
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
