using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.Models
{
    [Table("roles")]
    public class Roles
    {
        // ID роли. Первичный ключ.
        [Key]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("roles_id")]
        public int RolesId { get; set; }

        // Наименование роли пользователя.
        [RegularExpression(@"[а-яА-Яa-zA-Z""'\s-]*$", ErrorMessage = "Поле содержит недопустимые символы.")]
        [Required(ErrorMessage = "Требуется наименование роли пользователя.")]
        [StringLength(45)]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("roles_name")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Наименование роли пользователя")]
        public string RolesName { get; set; }

        // Флаг существования.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("roles_existed_flag")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Флаг существования")]
        public bool RolesExistedFlag { get; set; }

        // Таблица пользователей.
        public virtual List<Users> Users { get; set; }
    }
}