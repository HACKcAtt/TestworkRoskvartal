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
    public class CustomersController : Controller
    {
        private readonly DBContext _context;

        public CustomersController(DBContext context)
        {
            _context = context;
        }

        // GET: Doctors
        [Authorize(Roles = "Registrator, SimpleUser")]
        public async Task<IActionResult> Index(int? page, int? instancesPerPage, string searchString)
        {
            // Количество выводимых на одной странице профилей.
            int pageSize = (instancesPerPage ?? 5);
            if (pageSize == -1)
            {
                pageSize = _context.Customer.Count();
            }

            var query = _context.Customer.Include(c => c.Users).ThenInclude(u => u.UserRoles).Where(s => s.CustomerExistedFlag).OrderBy(p => p.Users.UsersName);
            //В случае, если поисковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Users.UsersName.Contains(searchString)).Where(s => s.CustomerExistedFlag).OrderBy(p => p.Users.UsersName);
            }
            // Реализация постраничного вывода профилей.
            // Номер страницы. Если он не получен в контроллер, то сделать его равным 1.
            int pageNumber = (page ?? 1);
            if (page > (int)Math.Ceiling(query.Count() / (double)pageSize))
            {
                pageNumber = (int)Math.Ceiling(query.Count() / (double)pageSize);
            }
            // Приведение возвращаемых профилей к виду пагинации с использованием самодельного класса PaginatedList<T>.
            PaginatedList<Customer> finalReturning = await PaginatedList<Customer>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилями.
            finalReturning.searchQuery = searchString;

            return View(finalReturning);
        }

        // Загрузка частичного представления - таблицы.
        [Authorize(Roles = "Registrator, SimpleUser")]
        public async Task<IActionResult> GetCustomersTable(int? page, int? instancesPerPage, string searchString, string sortOrder)
        {
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

            var query = _context.Customer.Include(c => c.Users).ThenInclude(u => u.UserRoles).Where(s => s.CustomerExistedFlag).OrderBy(p => p.Users.UsersName);
            //В случае, если посиковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Users.UsersName.Contains(searchString)).Where(s => s.CustomerExistedFlag).OrderBy(p => p.Users.UsersName);
            }

            switch (sortOrder)
            {
                case "name_descending":
                    query = query.OrderByDescending(q => q.Users.UsersName);
                    break;
                case "birthday_descending":
                    query = query.OrderByDescending(q => q.Users.UsersBirthday);
                    break;
                case "birthday_ascending":
                    query = query.OrderBy(q => q.Users.UsersBirthday);
                    break;
                default:
                    query = query.OrderBy(q => q.Users.UsersName);
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
            PaginatedList<Customer> finalReturning = await PaginatedList<Customer>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилями.
            finalReturning.searchQuery = searchString;
            return PartialView(finalReturning);
        }

        // GET: Customers/Create
        [Authorize(Roles = "Registrator, SimpleUser")]
        public IActionResult Create(int? id, int? userId)
        {
            if (id != null || userId != null)
            {
                if (userId != null)
                {
                    id = _context.Customer.FirstOrDefault(c => c.CustomerExistedFlag && c.UsersId == userId).CustomerId;
                }

                var customer = _context.Customer.Include(c => c.Users).ThenInclude(u => u.UserRoles).Where(s => s.CustomerExistedFlag).SingleOrDefault(m => m.CustomerId == id);
                CustomersViewModel instance = new CustomersViewModel();
                instance.Customers = customer;
                //instance.ClinicalDepartment = _context.ClinicalDepartment.Where(c => c.ClinicalDepartmentExistedFlag).ToList();
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
                    CustomersViewModel returnView = new CustomersViewModel();
                    returnView.Customers = new Customer();
                    returnView.Customers.Users = new Users();
                    returnView.Customers.Users.UserRoles = new Roles();
                    return PartialView(returnView);
                }
                // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
                else
                {
                    CustomersViewModel returnView = new CustomersViewModel();
                    returnView.Customers = new Customer();
                    returnView.Customers.Users = new Users();
                    returnView.Customers.Users.UserRoles = new Roles();
                    return View(returnView);
                }
            }
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Registrator, SimpleUser")]
        public IActionResult Create(CustomersViewModel customersViewModel, int? id, int? userId)
        {
            // Если в контроллер пришёл id, значит требуется правка, а не создание.
            if (id != null || userId != null)
            {
                // Если id не соответствуют, то, значит, такого нет, и возвращется HTTP-ответ с кодом 404 ("Не найдено").
                if (id != customersViewModel.Customers.CustomerId)
                {
                    return NotFound();
                }

                if (userId != null)
                {
                    id = _context.Customer.FirstOrDefault(c => c.CustomerExistedFlag && c.UsersId == userId).CustomerId;
                }

                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Отметка о том, что профиль активный (не удалённый).
                        customersViewModel.Customers.CustomerExistedFlag = true;
                        customersViewModel.Customers.Users.UsersExistedFlag = true;
                        customersViewModel.Customers.Users.RolesId = _context.Roles.FirstOrDefault(r => r.RolesName == "SimpleUser").RolesId;
                        Customer customer = customersViewModel.Customers;
                        Users user = customer.Users;
                        user.UsersId = _context.Users.AsNoTracking().FirstOrDefault(u => u.UsersId == customer.UsersId).UsersId;
                        // Занесение сделанных правок в базу данных.
                        _context.Update(user);
                        _context.Update(customer);
                        // Сохранение изменений в базе данных.
                        _context.SaveChanges();
                    }
                    // Проверка на конкурирующие запросы к базе данных.
                    catch (DbUpdateConcurrencyException)
                    {
                        // Если такого профиля нет, то возвращется HTTP-ответ с кодом 404 ("Не найдено").
                        if ((!InstanceExists(customersViewModel.Customers.CustomerId)) || (!customersViewModel.Customers.CustomerExistedFlag))
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
                return PartialView(customersViewModel);
            }
            else
            {
                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    if (_context.Customer.Any(d => d.Users.UsersPhoneNumber == customersViewModel.Customers.Users.UsersPhoneNumber || d.Users.UsersEmail == customersViewModel.Customers.Users.UsersEmail && d.Users.UsersExistedFlag))
                    {
                        return BadRequest();
                    }
                    // Отметка о том, что теперь профиль существует и активен.
                    customersViewModel.Customers.CustomerExistedFlag = true;
                    customersViewModel.Customers.Users.UsersExistedFlag = true;
                    customersViewModel.Customers.Users.RolesId = _context.Roles.FirstOrDefault(r => r.RolesName == "SimpleUser").RolesId;
                    Customer customer = customersViewModel.Customers;
                    Users user = customersViewModel.Customers.Users;
                    // Занесение сделанных правок в базу данных.
                    _context.Add(customer);
                    _context.Add(user);
                    _context.SaveChanges();
                    return new EmptyResult();
                }
                return PartialView(customersViewModel);
            }
        }

        // GET: Customers/Details/5
        [Authorize(Roles = "Registrator, SimpleUser")]
        public IActionResult Details(int? id, int? userId, CustomersViewModel customersView)
        {
            if (id == null && userId == null)
            {
                return NotFound();
            }
            if (userId != null)
            {
                id = _context.Customer.FirstOrDefault(c => c.CustomerExistedFlag && c.UsersId == userId).CustomerId;
            }
            var instance = _context.Customer.Include(с => с.Users).ThenInclude(u => u.UserRoles).Include(c => c.DoctorsAppointments).SingleOrDefault(m => m.CustomerId == id);
            var doctorAppointments = _context.DoctorsAppointments.Where(d => d.CustomerId == id);
            customersView.Doctors = new List<Doctors>();
            foreach (var item in doctorAppointments)
            {
                customersView.Doctors.Add(_context.Doctors.FirstOrDefault(d => d.DoctorsId == item.DoctorsId));
            }
            customersView.Customers = instance;
            customersView.customersOperationFlag = "Details";
            if ((instance == null) || (!instance.CustomerExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(customersView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View(customersView);
            }
        }

        // GET: ClinicalDepartments/Delete/5
        [Authorize(Roles = "Registrator")]
        public IActionResult Delete(int? id, CustomersViewModel customersView)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instance = _context.Customer.Include(с => с.Users).ThenInclude(u => u.UserRoles).SingleOrDefault(m => m.CustomerId == id);
            customersView.Customers = instance;
            customersView.customersOperationFlag = "Delete";
            if ((instance == null) || (!instance.CustomerExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", customersView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View("Details", customersView);
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Registrator")]
        public IActionResult Delete(int id)
        {
            var instance = _context.Customer.SingleOrDefault(m => m.CustomerId == id);
            instance.CustomerExistedFlag = false;
            Users user = _context.Users.AsNoTracking().SingleOrDefault(u => u.UsersId == instance.UsersId);
            user.UsersExistedFlag = false;
            _context.Update(instance);
            _context.Update(user);
            _context.SaveChanges();
            return new EmptyResult(); ;
        }

        // Проверка телефона на дупликацию.
        // POST: Customers/CheckInstanceName
        [Authorize(Roles = "Registrator, SimpleUser")]
        [HttpPost]
        public JsonResult CheckInstanceName(string instanceNameForCheck, int? instanceIdToPass)
        {
            // Флаг проверки.
            bool comparisonResultFlag = _context.Customer.Any(s => s.CustomerExistedFlag && s.Users.UsersPhoneNumber == instanceNameForCheck);
            // Флаг проверки: на случай, если полученные данные совпадают с самими собой в проверяемом профиле.
            bool theSameProfile = false;
            int instanceIdToCheck = -1;
            if (comparisonResultFlag)
            {
                instanceIdToCheck = _context.Customer.Where(s => s.CustomerExistedFlag && s.Users.UsersPhoneNumber == instanceNameForCheck).SingleOrDefault().CustomerId;
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

        // Проверка адреса электронной почты на дупликацию.
        // POST: Customers/CheckInstanceName
        [Authorize(Roles = "Registrator, SimpleUser")]
        [HttpPost]
        public JsonResult CheckInstanceEmail(string instanceEmailForCheck, int? instanceIdToPass)
        {
            // Флаг проверки.
            bool comparisonResultFlag = _context.Customer.Any(s => s.CustomerExistedFlag && s.Users.UsersEmail == instanceEmailForCheck);
            // Флаг проверки: на случай, если полученные данные совпадают с самими собой в проверяемом профиле.
            bool theSameProfile = false;
            int instanceIdToCheck = -1;
            if (comparisonResultFlag)
            {
                instanceIdToCheck = _context.Customer.Where(s => s.CustomerExistedFlag && s.Users.UsersEmail == instanceEmailForCheck).SingleOrDefault().CustomerId;
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
        private bool InstanceExists(int id)
        {
            return _context.Customer.Any(e => e.CustomerId == id);
        }
    }
}