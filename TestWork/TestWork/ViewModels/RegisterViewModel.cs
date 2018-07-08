using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestWork.Models;

namespace TestWork.ViewModels
{
    public class RegisterViewModel
    {
        public virtual Users Users { get; set; }

        // Пароль пользователя.
        [Required(ErrorMessage = "Требуется пароль")]
        [StringLength(32)]
        [DataType(DataType.Password)]
        // Атрибут  [Display(Name = "нужный_текст")] покажет при отображении представленияв названии столбца везде текст "нужный текст" вместо полученного из БД имени столбца.
        [Display(Name = "Пароль пользователя")]
        public string UsersPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("UsersPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
