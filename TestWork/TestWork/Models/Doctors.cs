using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.Models
{
    [Table("doctors")]
    public class Doctors
    {
        // Id доктора. Первичный ключ.
        [Key]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_id")]
        public int DoctorsId { get; set; }

        // Имя доктора.
        [RegularExpression(@"[а-яА-Яa-zA-Z""'\s-]*$", ErrorMessage = "Поле содержит недопустимые символы.")]
        [Required(ErrorMessage = "Требуется имя.")]
        [StringLength(100)]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_name")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Имя доктора")]
        public string DoctorsName { get; set; }

        // Специальность доктора.
        [StringLength(45)]
        [Required(ErrorMessage = "Требуется специальность доктора.")]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_specialization")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Специальность доктора")]
        public string DoctorsSpecialization { get; set; }

        // Номер телефона доктора.
        [Required(ErrorMessage = "Требуется номер телефона")]
        [RegularExpression(@"^[0-9\s+]*$", ErrorMessage = "Поле содержит недопустимые символы.")]
        [StringLength(20)]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_phone_number")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Телефонный номер доктора")]
        public string DoctorsPhoneNumber { get; set; }

        // Флаг существования.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("doctors_existed_flag")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Флаг существования")]
        public bool DoctorsExistedFlag { get; set; }

        // Id отделения.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Required(ErrorMessage = "Требуется отделение")]
        [Column("clinical_department_id")]
        public int ClinicalDepartmentId { get; set; }

        // Внешний ключ на Id отделения.
        [ForeignKey("ClinicalDepartmentId")]
        public ClinicalDepartment ClinicalDepartment { get; set; }

        // Таблица приёмов.
        public virtual List<DoctorsAppointments> DoctorsAppointments { get; set; }
    }
}
