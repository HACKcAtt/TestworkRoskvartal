using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
    public class CustomersIllnessesController : Controller
    {
        private readonly DBContext _context;

        public CustomersIllnessesController(DBContext context)
        {
            _context = context;
        }

        public static int customerId;

        // GET
        [Authorize(Roles = "Registrator")]
        public async Task<IActionResult> Index(int? page, int? instancesPerPage, string searchString, int customerIdd)
        {
            customerId = customerIdd;
            if (customerIdd == 0)
            {
                return BadRequest("Был получен пустой Id пациента.");
            }
            // Количество выводимых на одной странице профилей.
            int pageSize = (instancesPerPage ?? 5);
            if (pageSize == -1)
            {
                pageSize = _context.Customer.Count();
            }

            var query = _context.CustomersIllnesses.Include(c => c.Customer).ThenInclude(u => u.Users).Where(s => s.CustomersIllnessesrExistedFlag && s.CustomerId == customerIdd).OrderBy(p => p.CustomersIllnessesDateTimeOfAddition);
            ViewBag.CustomerName = _context.Customer.Include(u => u.Users).FirstOrDefault(c => c.CustomerExistedFlag && c.CustomerId == customerIdd).Users.UsersName;
            ViewBag.CustomerBirthday = _context.Customer.Include(u => u.Users).FirstOrDefault(c => c.CustomerExistedFlag && c.CustomerId == customerIdd).Users.UsersBirthday;
            ViewBag.CustomerId = customerIdd;
            //В случае, если поисковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.CustomersIllnessesName.Contains(searchString)).Where(s => s.CustomersIllnessesrExistedFlag && s.CustomerId == customerIdd).OrderBy(p => p.CustomersIllnessesDateTimeOfAddition);
            }
            // Реализация постраничного вывода профилей.
            // Номер страницы. Если он не получен в контроллер, то сделать его равным 1.
            int pageNumber = (page ?? 1);
            if (page > (int)Math.Ceiling(query.Count() / (double)pageSize))
            {
                pageNumber = (int)Math.Ceiling(query.Count() / (double)pageSize);
            }
            // Приведение возвращаемых профилей к виду пагинации с использованием самодельного класса PaginatedList<T>.
            PaginatedList<CustomersIllnesses> finalReturning = await PaginatedList<CustomersIllnesses>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилями.
            finalReturning.searchQuery = searchString;

            return View(finalReturning);
        }

        // Загрузка частичного представления - таблицы.
        [Authorize(Roles = "Registrator")]
        public async Task<IActionResult> GetCustomersIllnessesTable(int? page, int? instancesPerPage, string searchString, string sortOrder, int customerIdd)
        {
            customerIdd = customerId;
            // Количество выводимых на одной странице профилей.
            int pageSize = (instancesPerPage ?? 5);
            if (pageSize == -1)
            {
                pageSize = _context.Customer.Count();
            }

            // Параметры сортировки.
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_descending" : "";
            ViewBag.BirthdaySortParm = sortOrder == "birthday_ascending" ? "birthday_descending" : "birthday_ascending";

            var query = _context.CustomersIllnesses.Include(c => c.Customer).ThenInclude(u => u.Users).Where(s => s.CustomersIllnessesrExistedFlag && s.CustomerId == customerIdd).OrderBy(p => p.CustomersIllnessesDateTimeOfAddition);
            ViewBag.CustomerName = _context.Customer.Include(u => u.Users).FirstOrDefault(c => c.CustomerExistedFlag && c.CustomerId == customerIdd).Users.UsersName;
            ViewBag.CustomerBirthday = _context.Customer.Include(u => u.Users).FirstOrDefault(c => c.CustomerExistedFlag && c.CustomerId == customerIdd).Users.UsersBirthday;
            ViewBag.CustomerId = customerIdd;
            //В случае, если посиковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.CustomersIllnessesName.Contains(searchString)).Where(s => s.CustomersIllnessesrExistedFlag && s.CustomerId == customerIdd).OrderBy(p => p.CustomersIllnessesDateTimeOfAddition);
            }

            switch (sortOrder)
            {
                case "name_descending":
                    query = query.OrderByDescending(q => q.CustomersIllnessesName);
                    break;
                case "birthday_descending":
                    query = query.OrderByDescending(q => q.CustomersIllnessesDateTimeOfAddition);
                    break;
                case "birthday_ascending":
                    query = query.OrderBy(q => q.CustomersIllnessesDateTimeOfAddition);
                    break;
                default:
                    query = query.OrderBy(q => q.CustomersIllnessesName);
                    break;
            }

            // Реализация постраничного вывода профилей.
            // Номер страницы. Если он не получен в контроллер, то сделать его равным 1.
            int pageNumber = (page ?? 1);
            if (page > (int)Math.Ceiling(query.Count() / (double)pageSize))
            {
                pageNumber = (int)Math.Ceiling(query.Count() / (double)pageSize);
            }
            // Приведение возвращаемых профилей к виду пагинации с использованием самодельного класса PaginatedList<T>.
            PaginatedList<CustomersIllnesses> finalReturning = await PaginatedList<CustomersIllnesses>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилями.
            finalReturning.searchQuery = searchString;
            return PartialView(finalReturning);
        }

        // GET: /Create
        [Authorize(Roles = "Registrator")]
        public IActionResult Create(int? id, int customerIdd)
        {
            if (customerIdd == 0)
            {
                return BadRequest("Был получен пустой Id пациента.");
            }
            ViewBag.CustomerName = _context.Customer.Include(u => u.Users).FirstOrDefault(c => c.CustomerExistedFlag && c.CustomerId == customerIdd).Users.UsersName;
            ViewBag.CustomerBirthday = _context.Customer.Include(u => u.Users).FirstOrDefault(c => c.CustomerExistedFlag && c.CustomerId == customerIdd).Users.UsersBirthday;
            ViewBag.CustomerId = customerIdd;
            if (id != null)
            {
                var illness = _context.CustomersIllnesses.Include(c => c.Customer).ThenInclude(u => u.Users).Where(c => c.CustomersIllnessesrExistedFlag && c.CustomerId == customerIdd).SingleOrDefault(c => c.CustomersIllnessesId == id);
                CustomersIllnesesViewModel instance = new CustomersIllnesesViewModel();
                instance.CustomersIllness = illness;
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
                    CustomersIllnesesViewModel returnView = new CustomersIllnesesViewModel();
                    returnView.CustomersIllness = new CustomersIllnesses();
                    returnView.Customers = new Customer();
                    returnView.Customers.Users = new Users();
                    returnView.Customers.Users.UserRoles = new Roles();
                    return PartialView(returnView);
                }
                // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
                else
                {
                    CustomersIllnesesViewModel returnView = new CustomersIllnesesViewModel();
                    returnView.CustomersIllness = new CustomersIllnesses();
                    returnView.Customers = new Customer();
                    returnView.Customers.Users = new Users();
                    returnView.Customers.Users.UserRoles = new Roles();
                    return View(returnView);
                }
            }
        }

        // POST: /Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Registrator")]
        public IActionResult Create(CustomersIllnesesViewModel customersIlnessViewModel, int? id, int customerIdd)
        {
            if (customersIlnessViewModel.customerIdd == 0)
            {
                return BadRequest("Был получен пустой Id пациента.");
            }
            // Если в контроллер пришёл id, значит требуется правка, а не создание.
            if (id != null)
            {
                // Если id не соответствуют, то, значит, такого нет, и возвращется HTTP-ответ с кодом 404 ("Не найдено").
                if (id != customersIlnessViewModel.CustomersIllness.CustomersIllnessesId)
                {
                    return NotFound();
                }

                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Отметка о том, что профиль активный (не удалённый).
                        customersIlnessViewModel.CustomersIllness.CustomersIllnessesrExistedFlag = true;
                        customersIlnessViewModel.CustomersIllness.CustomersIllnessesDateTimeOfAddition = DateTime.Now.ToString();
                        customersIlnessViewModel.CustomersIllness.CustomerId = customersIlnessViewModel.customerIdd;
                        CustomersIllnesses customerIllness = customersIlnessViewModel.CustomersIllness;
                        // Занесение сделанных правок в базу данных.
                        _context.Update(customerIllness);
                        // Сохранение изменений в базе данных.
                        _context.SaveChanges();
                    }
                    // Проверка на конкурирующие запросы к базе данных.
                    catch (DbUpdateConcurrencyException)
                    {
                        // Если такого профиля нет, то возвращется HTTP-ответ с кодом 404 ("Не найдено").
                        if ((!InstanceExists(customersIlnessViewModel.CustomersIllness.CustomersIllnessesId)) || (!customersIlnessViewModel.CustomersIllness.CustomersIllnessesrExistedFlag))
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
                return PartialView(customersIlnessViewModel);
            }
            else
            {
                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    // Отметка о том, что теперь профиль существует и активен.
                    customersIlnessViewModel.CustomersIllness.CustomersIllnessesrExistedFlag = true;
                    customersIlnessViewModel.CustomersIllness.CustomersIllnessesDateTimeOfAddition = DateTime.Now.ToString();
                    customersIlnessViewModel.CustomersIllness.CustomerId = customersIlnessViewModel.customerIdd;
                    CustomersIllnesses customerIllness = customersIlnessViewModel.CustomersIllness;
                    // Занесение сделанных правок в базу данных.
                    _context.Add(customerIllness);
                    _context.SaveChanges();
                    return new EmptyResult();
                }
                return PartialView(customersIlnessViewModel);
            }
        }

        // GET: Customers/Details/5
        [Authorize(Roles = "Registrator")]
        public IActionResult Details(int? id, CustomersIllnesesViewModel customerIlnessView)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instance = _context.CustomersIllnesses.Include(с => с.Customer).ThenInclude(u => u.Users).SingleOrDefault(m => m.CustomersIllnessesId == id);
            customerIlnessView.CustomersIllness = instance;
            customerIlnessView.customersOperationFlag = "Details";
            if ((instance == null) || (!instance.CustomersIllnessesrExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(customerIlnessView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View(customerIlnessView);
            }
        }

        // GET: ClinicalDepartments/Delete/5
        [Authorize(Roles = "Registrator")]
        public IActionResult Delete(int? id, CustomersIllnesesViewModel customerIlnessView)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instance = _context.CustomersIllnesses.Include(с => с.Customer).ThenInclude(u => u.Users).SingleOrDefault(m => m.CustomersIllnessesId == id);
            customerIlnessView.CustomersIllness = instance;
            customerIlnessView.customersOperationFlag = "Delete";
            if ((instance == null) || (!instance.CustomersIllnessesrExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", customerIlnessView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View("Details", customerIlnessView);
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Registrator")]
        public IActionResult Delete(int id)
        {
            var instance = _context.CustomersIllnesses.SingleOrDefault(m => m.CustomersIllnessesId == id);
            instance.CustomersIllnessesrExistedFlag = false;
            _context.Update(instance);
            _context.SaveChanges();
            return new EmptyResult(); ;
        }

        [Authorize(Roles = "Registrator")]
        private bool InstanceExists(int id)
        {
            return _context.Customer.Any(e => e.CustomerId == id);
        }
    }
}