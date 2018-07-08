using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.Models
{
    [Table("clinical_department")]
    public class ClinicalDepartment
    {
        // Id отделения. Первичный ключ.
        [Key]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("clinical_department_id")]
        public int ClinicalDepartmentId { get; set; }

        // Наименование отделения.
        [Required(ErrorMessage = "Требуется наименование отделения.")]
        [StringLength(45)]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("clinical_department_name")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Наименование отделения")]
        public string ClinicalDepartmentName { get; set; }

        // Флаг существования.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("clinical_department_existed_flag")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Флаг существования")]
        public bool ClinicalDepartmentExistedFlag { get; set; }

        // Таблица докторов.
        public virtual List<Doctors> Doctors { get; set; }
    }
}
