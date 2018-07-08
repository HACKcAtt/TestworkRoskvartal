using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestWork.Models;
using TestWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TestWork.Controllers
{
    public class ClinicalDepartmentsController : Controller
    {
        private readonly DBContext _context;

        public ClinicalDepartmentsController(DBContext context)
        {
            _context = context;
        }

        // GET: ClinicalDepartments
        [Authorize(Roles = "Registrator, SimpleUser")]
        public async Task<IActionResult> Index(int? page, int? instancesPerPage, string searchString, string sortOrder)
        {
            // Количество выводимых на одной странице профилей.
            int pageSize = (instancesPerPage ?? 5);
            if (pageSize == -1)
            {
                pageSize = _context.ClinicalDepartment.Count();
            }

            // Параметры сортировки.
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_descending" : "";

            var query = _context.ClinicalDepartment.Where(s => s.ClinicalDepartmentExistedFlag).OrderBy(p => p.ClinicalDepartmentName);
            //В случае, если поисковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.ClinicalDepartmentName.Contains(searchString)).Where(s => s.ClinicalDepartmentExistedFlag).OrderBy(p => p.ClinicalDepartmentName);
            }

            switch (sortOrder)
            {
                case "name_descending":
                    query = query.OrderByDescending(q => q.ClinicalDepartmentName);
                    break;
                default:
                    query = query.OrderBy(q => q.ClinicalDepartmentName);
                    break;
            }

            // Реализация постраничного вывода профилей.
            // Номер страницы. Если он не получен в контроллер, то сделать его равным 1.
            int pageNumber = (page ?? 1);
            if (page > (int)Math.Ceiling(query.Count() / (double)pageSize))
            {
                pageNumber = (int)Math.Ceiling(query.Count() / (double)pageSize);
            }
            // Приведение возвращаемых отделений к виду пагинации с использованием самодельного класса PaginatedList<T>.
            PaginatedList<ClinicalDepartment> finalReturning = await PaginatedList<ClinicalDepartment>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилям.
            finalReturning.searchQuery = searchString;

            return View(finalReturning);
        }

        // Загрузка частичного представления - таблицы.
        [Authorize(Roles = "Registrator, SimpleUser")]
        public async Task<IActionResult> GetClinicalDepartmentsTable(int? page, int? instancesPerPage, string searchString, string sortOrder)
        {
            // Количество выводимых на одной странице профилей.
            int pageSize = (instancesPerPage ?? 5);
            if (pageSize == -1)
            {
                pageSize = _context.ClinicalDepartment.Count();
            }

            // Параметры сортировки.
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_descending" : "";

            var query = _context.ClinicalDepartment.Where(s => s.ClinicalDepartmentExistedFlag).OrderBy(p => p.ClinicalDepartmentName);
            //В случае, если посиковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.ClinicalDepartmentName.Contains(searchString)).Where(s => s.ClinicalDepartmentExistedFlag).OrderBy(p => p.ClinicalDepartmentName);
            }

            switch (sortOrder)
            {
                case "name_descending":
                    query = query.OrderByDescending(q => q.ClinicalDepartmentName);
                    break;
                default:
                    query = query.OrderBy(q => q.ClinicalDepartmentName);
                    break;
            }

            // Реализация постраничного вывода профилей.
            // Номер страницы. Если он не получен в контроллер, то сделать его равным 1.
            int pageNumber = (page ?? 1);
            if (page > (int)Math.Ceiling(query.Count() / (double)pageSize))
            {
                pageNumber = (int)Math.Ceiling(query.Count() / (double)pageSize);
            }
            // Приведение возвращаемых отделений к виду пагинации с использованием самодельного класса PaginatedList<T>.
            PaginatedList<ClinicalDepartment> finalReturning = await PaginatedList<ClinicalDepartment>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилям.
            finalReturning.searchQuery = searchString;
            return PartialView(finalReturning);
        }

        // GET: ClinicalDepartments/Create
        [Authorize(Roles = "Registrator")]
        public IActionResult Create(int? id)
        {
            if (id != null)
            {
                var instance = _context.ClinicalDepartment.Where(s => s.ClinicalDepartmentExistedFlag).SingleOrDefault(m => m.ClinicalDepartmentId == id);
                // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(instance);
                }
                // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
                else
                {
                    return View(instance);
                }
            }
            else
            {
                // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(new ClinicalDepartment());
                }
                // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
                else
                {
                    return View(new ClinicalDepartment());
                }
            }
        }

        // POST: ClinicalDepartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Registrator")]
        public IActionResult Create(ClinicalDepartment clinicalDepartment, int? id)
        {
            // Если в контроллер пришёл id, значит требуется правка, а не создание.
            if (id != null)
            {
                // Если id не соответствуют, то, значит, такого нет, и возвращется HTTP-ответ с кодом 404 ("Не найдено").
                if (id != clinicalDepartment.ClinicalDepartmentId)
                {
                    return NotFound();
                }

                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Отметка о том, что профиль активный (не удалённый).
                        clinicalDepartment.ClinicalDepartmentExistedFlag = true;
                        // Занесение сделанных правок в базу данных.
                        _context.Update(clinicalDepartment);
                        // Сохранение изменений в базе данных.
                        _context.SaveChanges();
                    }
                    // Проверка на конкурирующие запросы к базе данных.
                    catch (DbUpdateConcurrencyException)
                    {
                        // Если такого профиля нет, то возвращется HTTP-ответ с кодом 404 ("Не найдено").
                        if ((!ClinicalDepartmentExists(clinicalDepartment.ClinicalDepartmentId)) || (!clinicalDepartment.ClinicalDepartmentExistedFlag))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return new EmptyResult();
                }
                return PartialView(clinicalDepartment);
            }
            else
            {
                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    if (_context.ClinicalDepartment.Any(d => d.ClinicalDepartmentName == clinicalDepartment.ClinicalDepartmentName && d.ClinicalDepartmentExistedFlag))
                    {
                        return BadRequest();
                    }
                    // Отметка о том, что теперь профиль существует и активен.
                    clinicalDepartment.ClinicalDepartmentExistedFlag = true;
                    _context.Add(clinicalDepartment);
                    _context.SaveChanges();
                    return new EmptyResult();
                }
                return PartialView(clinicalDepartment);
            }
        }

        // GET: Customers/Details/5
        [Authorize(Roles = "Registrator, SimpleUser")]
        public IActionResult Details(int? id, ClinicalDepartmentDetailsDeleteViewModel clinicalDepartmentDetailsDeleteView)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instance = _context.ClinicalDepartment.Include(d => d.Doctors).SingleOrDefault(m => m.ClinicalDepartmentId == id);
            clinicalDepartmentDetailsDeleteView.ClinicalDepartment = instance;
            clinicalDepartmentDetailsDeleteView.clinicalDepartmentOperationFlag = "Details";
            if ((instance == null) || (!instance.ClinicalDepartmentExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(clinicalDepartmentDetailsDeleteView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View(clinicalDepartmentDetailsDeleteView);
            }
        }

        // GET: ClinicalDepartments/Delete/5
        [Authorize(Roles = "Registrator")]
        public IActionResult Delete(int? id, ClinicalDepartmentDetailsDeleteViewModel clinicalDepartmentDetailsDeleteView)
        {
            if (id == null)
            {
                return NotFound();
            }
            var clinicalDepartment = _context.ClinicalDepartment.SingleOrDefault(m => m.ClinicalDepartmentId == id);
            clinicalDepartmentDetailsDeleteView.ClinicalDepartment = clinicalDepartment;
            clinicalDepartmentDetailsDeleteView.clinicalDepartmentOperationFlag = "Delete";
            if ((clinicalDepartment == null) || (!clinicalDepartment.ClinicalDepartmentExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", clinicalDepartmentDetailsDeleteView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View("Details", clinicalDepartmentDetailsDeleteView);
            }
        }

        // POST: ClinicalDepartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Registrator")]
        public IActionResult DeleteConfirmed(int id)
        {
            var instance = _context.ClinicalDepartment.SingleOrDefault(m => m.ClinicalDepartmentId == id);
            instance.ClinicalDepartmentExistedFlag = false;
            _context.Update(instance);
            _context.SaveChanges();
            return new EmptyResult(); ;
        }

        // Проверка отделения на дупликацию.
        // POST: ClinicalDepartments/CheckInstanceName
        [HttpPost]
        [Authorize(Roles = "Registrator, SimpleUser")]
        public JsonResult CheckInstanceName(string instanceNameForCheck, int? instanceIdToPass)
        {
            // Флаг проверки.
            bool comparisonResultFlag = _context.ClinicalDepartment.Any(s => s.ClinicalDepartmentExistedFlag && s.ClinicalDepartmentName == instanceNameForCheck);
            // Флаг проверки: на случай, если полученное имя совпадает с самим собой в проверяемом профиле.
            bool theSameProfile = false;
            int instanceIdToCheck = -1;
            if (comparisonResultFlag)
            {
                instanceIdToCheck = _context.ClinicalDepartment.Where(s => s.ClinicalDepartmentExistedFlag && s.ClinicalDepartmentName == instanceNameForCheck).SingleOrDefault().ClinicalDepartmentId;
            }
            else
            {
                return Json(0);
            }
            
            if (instanceIdToPass == instanceIdToCheck)
            {
                theSameProfile = true;
            }
            // Если результат проверки показал совпадение и профили разные, то вернуть на клиент 1.
            if ((comparisonResultFlag) && (!theSameProfile))
            {
                return Json(1);
            }
            // Если результат проверки не показал совпадение, или профиль один и тот же, то вернуть на клиент 0.
            else
            {
                return Json(0);
            }
        }

        [Authorize(Roles = "Registrator, SimpleUser")]
        private bool ClinicalDepartmentExists(int id)
        {
            return _context.ClinicalDepartment.Any(e => e.ClinicalDepartmentId == id);
        }
    }
}
