using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWork.Models
{
    [Table("customers_illnesses")]
    public class CustomersIllnesses
    {
        // Id заболевания пациента. Первичный ключ.
        [Key]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("customers_illnesses_id")]
        public int CustomersIllnessesId { get; set; }

        // Наименование заболевания пациента.
        [Required(ErrorMessage = "Требуется наименование заболевания пациента")]
        [StringLength(100)]
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("customers_illnesses_name")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Наименование заболевания")]
        public string CustomersIllnessesName { get; set; }

        // Запись по болезни.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("customers_illnesses_description")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Запись по болезни")]
        public string CustomersIllnessesDescription { get; set; }

        // Дата и время записи по болезни.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("customers_illnesses_datetime_of_addition")]
        [DataType(DataType.DateTime, ErrorMessage = "Поле содержит недопустимые символы.")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Дата и время записи по болезни")]
        public string CustomersIllnessesDateTimeOfAddition { get; set; }

        // Флаг существования.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("customers_illnesses_existed_flag")]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Флаг существования")]
        public bool CustomersIllnessesrExistedFlag { get; set; }

        // Id пользователя.
        // Атрибут [Column("название_столбца_в_таблице_бд")] необходим в случае отличия названия столбца в модели (непосредственно здесь) от его непосредственного названия в БД.
        [Column("customer_id")]
        public int CustomerId { get; set; }

        // Внешний ключ на Id пользователя.
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
