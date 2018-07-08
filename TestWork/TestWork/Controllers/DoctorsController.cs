using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestWork.ViewModels;
using TestWork.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace TestWork.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly DBContext _context;

        public DoctorsController(DBContext context)
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
                pageSize = _context.Doctors.Count();
            }

            var query = _context.Doctors.Include(d => d.ClinicalDepartment).Where(s => s.DoctorsExistedFlag).OrderBy(p => p.DoctorsName);
            //В случае, если поисковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.DoctorsName.Contains(searchString)).Where(s => s.DoctorsExistedFlag).OrderBy(p => p.DoctorsName);
            }
            // Реализация постраничного вывода профилей.
            // Номер страницы. Если он не получен в контроллер, то сделать его равным 1.
            int pageNumber = (page ?? 1);
            if (page > (int)Math.Ceiling(query.Count() / (double)pageSize))
            {
                pageNumber = (int)Math.Ceiling(query.Count() / (double)pageSize);
            }
            // Приведение возвращаемых профилей к виду пагинации с использованием самодельного класса PaginatedList<T>.
            PaginatedList<Doctors> finalReturning = await PaginatedList<Doctors>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилями.
            finalReturning.searchQuery = searchString;

            return View(finalReturning);
        }

        // Загрузка частичного представления - таблицы.
        [Authorize(Roles = "Registrator, SimpleUser")]
        public async Task<IActionResult> GetDoctorsTable(int? page, int? instancesPerPage, string searchString, string sortOrder)
        {
            // Количество выводимых на одной странице профилей.
            int pageSize = (instancesPerPage ?? 5);
            if (pageSize == -1)
            {
                pageSize = _context.Doctors.Count();
            }

            // Параметры сортировки.
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_descending" : "";
            ViewBag.ClinicalDepartmentNameSortParm = sortOrder == "clinical_department_name_ascending" ? "clinical_department_name_descending" : "clinical_department_name_ascending";

            var query = _context.Doctors.Include(d => d.ClinicalDepartment).Where(s => s.DoctorsExistedFlag).OrderBy(p => p.DoctorsName);
            //В случае, если посиковая строка не пуста, делать выборку в соответствии с искомыми данными.
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.DoctorsName.Contains(searchString)).Where(s => s.DoctorsExistedFlag).OrderBy(p => p.DoctorsName);
            }

            switch (sortOrder)
            {
                case "name_descending":
                    query = query.OrderByDescending(q => q.DoctorsName);
                    break;
                case "clinical_department_name_descending":
                    query = query.OrderByDescending(q => q.ClinicalDepartment.ClinicalDepartmentName);
                    break;
                case "clinical_department_name_ascending":
                    query = query.OrderBy(q => q.ClinicalDepartment.ClinicalDepartmentName);
                    break;
                default:
                    query = query.OrderBy(q => q.DoctorsName);
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
            PaginatedList<Doctors> finalReturning = await PaginatedList<Doctors>.CreateAsync(query, pageNumber, pageSize);
            // Применение поиска к выводимым профилями.
            finalReturning.searchQuery = searchString;
            return PartialView(finalReturning);
        }

        // GET: Doctors/Create
        [Authorize(Roles = "Registrator")]
        public IActionResult Create(int? id)
        {
            if (id != null)
            {
                var doctor = _context.Doctors.Include(d => d.ClinicalDepartment).Include(d => d.DoctorsAppointments).Where(s => s.DoctorsExistedFlag).SingleOrDefault(m => m.DoctorsId == id);
                DoctorsViewModel instance = new DoctorsViewModel();
                instance.Doctor = doctor;
                instance.ClinicalDepartments = _context.ClinicalDepartment.Where(c => c.ClinicalDepartmentExistedFlag).ToList();
                instance.DoctorsAppointmentss = _context.DoctorsAppointments.Where(d => d.DoctorAppointmentsExistedFlag && d.DoctorsId == id).ToList();
                if (instance.DoctorsAppointmentss.Any())
                {
                    instance.doctorsStartDate = (Convert.ToDateTime(instance.DoctorsAppointmentss.First().DoctorAppointmentsDate)).Date.ToShortDateString();
                    instance.doctorsStopDate = (Convert.ToDateTime(instance.DoctorsAppointmentss.Last().DoctorAppointmentsDate)).Date.ToShortDateString();
                }
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
                    DoctorsViewModel returnView = new DoctorsViewModel();
                    returnView.Doctor = new Doctors();
                    returnView.ClinicalDepartments = _context.ClinicalDepartment.Where(c => c.ClinicalDepartmentExistedFlag).ToList();
                    return PartialView(returnView);
                }
                // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
                else
                {
                    DoctorsViewModel returnView = new DoctorsViewModel();
                    returnView.Doctor = new Doctors();
                    returnView.ClinicalDepartments = _context.ClinicalDepartment.Where(c => c.ClinicalDepartmentExistedFlag).ToList();
                    return View(returnView);
                }
            }
        }

        // Заполнение графика доктора (пока что упрощённо - отрезками времени по 20 минут с 09:00 до 18:00 в каждый из дней за указанный период окромя субботы и воскресенья).
        [Authorize(Roles = "Registrator")]
        public bool MakeDoctorsWorktime (DoctorsViewModel doctors, int? id)
        {
            // Получение id только что сохранённого доктора.
            int idD;
            if (id == null)
            {
                idD = _context.Doctors.FirstOrDefault(d => d.DoctorsExistedFlag && d.DoctorsName == doctors.Doctor.DoctorsName && d.DoctorsPhoneNumber == doctors.Doctor.DoctorsPhoneNumber && d.ClinicalDepartmentId == doctors.Doctor.ClinicalDepartmentId && d.DoctorsSpecialization == doctors.Doctor.DoctorsSpecialization).DoctorsId;
            }
            else
            {
                idD = (int)id;
            }

            DateTime startDate = Convert.ToDateTime(doctors.doctorsStartDate);
            DateTime stopDate = Convert.ToDateTime(doctors.doctorsStopDate);
            var datesList = new List<DateTime>();
            // Количество периодов приёма пациентов в рабочий день. Если шаг по 20 минут при рабочем дне с 09:00 до 18:00, то в час возможно 3 приёма, а за 9 часов, соответственно, возможно 26 приёмов.
            int minutePeriodsPerDay = 26;

            if (Convert.ToDateTime(doctors.doctorsStopDate) >= Convert.ToDateTime(doctors.doctorsStartDate))
            {
                double daysToAdd = 1;
                while (startDate <= stopDate)
                {
                    if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        datesList.Add(startDate);
                    }
                    startDate = startDate.AddDays(daysToAdd);
                }
                List<DoctorsAppointments> doctorsAppointmentsList = new List<DoctorsAppointments>();

                // Формирование разбиения рабочего дня на отрезки по 20 минут.
                // Начало рабочего дня
                string startTime = "09:00";
                DateTime start = Convert.ToDateTime(startTime);
                List<DateTime> timing = new List<DateTime>();
                timing.Add(start);
                List<string> minutePeriodsPerDayArray = new List<string>();
                // Если рабочий день с 09:00 по 18:00, то в нём 26 отрезков по 20 минут.
                for (int i = 0; i <= 26; i++)
                {
                    minutePeriodsPerDayArray.Add(timing[i].ToString("HH:mm"));
                    if (i == 26)
                    {
                        break;
                    }
                    timing.Add(timing[i].AddMinutes(20));
                }

                // Список отрезков времени работы доктора, в которые уже записаны пациенты.
                List<DoctorsAppointments> alreadyExistedDoctorsAppointmentsListWithAppointedCustomers = _context.DoctorsAppointments.Where(d => d.DoctorAppointmentsExistedFlag && d.DoctorsId == idD && d.CustomerId != null).ToList();

                // Создаётся график приёмов. Приём (отрезок времени в 20 минут на него) считается свободным, если в данном DoctorsAppointments (приёме) CustomerId равен null, то есть никакой пациент ещё не записался на этот отрезок времени. 
                foreach (var date in datesList)
                {

                    for (int i = 0; i < minutePeriodsPerDay; i++)
                    {
                        // Если на текущий отрезок времени уже был записан пациент, и этот отрезок времени попадает в рамки создаваемого графика, то внести id данного пациента в список изменения или добавления графика доктора, а если нет, то просто создать данный отрезок времени без какого-либо записанного на него пациента.
                        if (alreadyExistedDoctorsAppointmentsListWithAppointedCustomers.Any(d => d.DoctorAppointmentsDate == date.ToShortDateString() && d.DoctorAppointmentsTime == minutePeriodsPerDayArray[i]))
                        {
                            doctorsAppointmentsList.Add(new DoctorsAppointments
                            {
                                DoctorsId = idD,
                                CustomerId = alreadyExistedDoctorsAppointmentsListWithAppointedCustomers.FirstOrDefault(d => d.DoctorAppointmentsDate == date.ToShortDateString() && d.DoctorAppointmentsTime == minutePeriodsPerDayArray[i]).CustomerId,
                                DoctorAppointmentsDate = date.ToString("yyyy-MM-dd"),
                                DoctorAppointmentsTime = minutePeriodsPerDayArray[i],
                                DoctorAppointmentsExistedFlag = true
                            });
                        }
                        else
                        {
                            doctorsAppointmentsList.Add(new DoctorsAppointments
                            {
                                DoctorsId = idD,
                                DoctorAppointmentsDate = date.ToString("yyyy-MM-dd"),
                                DoctorAppointmentsTime = minutePeriodsPerDayArray[i],
                                DoctorAppointmentsExistedFlag = true
                            });
                        }
                    }
                }

                // Если график доктора уже существовал, то удалить старый график и внести новый (но с сохранением записей пациентов, запись которых попадает по дате в рамки нового графика). Если график доктора ещё не был создан, то просто добавить его.
                if (doctors.Doctor.DoctorsAppointments != doctorsAppointmentsList && doctors.Doctor.DoctorsAppointments != null)
                {
                    _context.DoctorsAppointments.RemoveRange(doctors.Doctor.DoctorsAppointments);
                    _context.AddRange(doctorsAppointmentsList);
                    _context.SaveChanges();
                }
                if (doctors.Doctor.DoctorsAppointments == null)
                {
                    _context.AddRange(doctorsAppointmentsList);
                    _context.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Registrator")]
        public IActionResult Create(DoctorsViewModel doctors, int? id)
        {
            // Если в контроллер пришёл id, значит требуется правка, а не создание.
            if (id != null)
            {
                // Если id не соответствуют, то, значит, такого нет, и возвращется HTTP-ответ с кодом 404 ("Не найдено").
                if (id != doctors.Doctor.DoctorsId)
                {
                    return NotFound();
                }
                
                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Отметка о том, что профиль активный (не удалённый).
                        doctors.Doctor.DoctorsExistedFlag = true;
                        //doctors.Doctor.ClinicalDepartmentId = _context.ClinicalDepartment.FirstOrDefault(d => d.ClinicalDepartmentExistedFlag && d.ClinicalDepartmentName == doctors.Doctor.ClinicalDepartment.ClinicalDepartmentName).ClinicalDepartmentId;
                        // Занесение сделанных правок в базу данных.
                        _context.Update(doctors.Doctor);
                        // Сохранение изменений в базе данных.
                        _context.SaveChanges();

                        bool resultOfMakeDoctorsWorktimeMethod = MakeDoctorsWorktime(doctors, id);

                        if (!resultOfMakeDoctorsWorktimeMethod)
                        { 
                           return BadRequest("Дата окончания периода работы доктора находится по календарю раньше, чем дата начала периода работы доктора, что невозможно");
                        }
                    }
                    // Проверка на конкурирующие запросы к базе данных.
                    catch (DbUpdateConcurrencyException)
                    {
                        // Если такого профиля нет, то возвращется HTTP-ответ с кодом 404 ("Не найдено").
                        if ((!InstanceExists(doctors.Doctor.DoctorsId)) || (!doctors.Doctor.DoctorsExistedFlag))
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
                return PartialView(doctors);
            }
            else
            {
                // Проверка модели на правильность.
                if (ModelState.IsValid)
                {
                    doctors.Doctor.DoctorsExistedFlag = true;
                    _context.Add(doctors.Doctor);
                    _context.SaveChanges();

                    bool resultOfMakeDoctorsWorktimeMethod = MakeDoctorsWorktime(doctors, id);

                    if (!resultOfMakeDoctorsWorktimeMethod)
                    {
                        return BadRequest("Дата окончания периода работы доктора находится по календарю раньше, чем дата начала периода работы доктора, что невозможно");
                    }
                    return new EmptyResult();
                }
                doctors.ClinicalDepartments = _context.ClinicalDepartment.Where(c => c.ClinicalDepartmentExistedFlag).ToList();
                return PartialView(doctors);
            }
        }

        // GET: Doctors/Details/5
        [Authorize(Roles = "Registrator, SimpleUser")]
        public IActionResult Details(int? id, DoctorsViewModel doctorsView)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instance = _context.Doctors.Include(d => d.ClinicalDepartment).SingleOrDefault(m => m.DoctorsId == id);
            doctorsView.Doctor = instance;
            doctorsView.doctorsOperationFlag = "Details";
            //doctorsView.doctorsOperationFlag = "Details";
            if ((instance == null) || (!instance.DoctorsExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(doctorsView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View(doctorsView);
            }
        }

        // GET: Doctors/Delete/5
        [Authorize(Roles = "Registrator")]
        public IActionResult Delete(int? id, DoctorsViewModel doctorsView)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instance = _context.Doctors.SingleOrDefault(m => m.DoctorsId == id);
            doctorsView.Doctor = instance;
            doctorsView.doctorsOperationFlag = "Delete";
            //doctorsView.doctorsOperationFlag = "Delete";
            if ((instance == null) || (!instance.DoctorsExistedFlag))
            {
                return NotFound();
            }
            // В случае вызова формы создания профиля модальным окном возвращать частичное представление.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", doctorsView);
            }
            // В случае вызова формы создания профиля отдельным окном (отдельной вкладкой) возвращать полное представление.
            else
            {
                return View("Details", doctorsView);
            }
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Registrator")]
        public IActionResult Delete(int id)
        {
            var instance = _context.Doctors.SingleOrDefault(m => m.DoctorsId == id);
            instance.DoctorsExistedFlag = false;
            _context.Update(instance);
            _context.SaveChanges();
            return new EmptyResult(); ;
        }

        // GET: Doctors/Create
        [Authorize(Roles = "Registrator, SimpleUser")]
        public IActionResult CreateDoctorsAppointment(int id, string date)
        {
            var doctor = _context.Doctors.Include(d => d.ClinicalDepartment).Include(d => d.DoctorsAppointments).Where(s => s.DoctorsExistedFlag).SingleOrDefault(m => m.DoctorsId == id);
            DoctorsViewModel instance = new DoctorsViewModel();
            instance.DoctorsAppointment = new DoctorsAppointments();
            instance.Doctor = doctor;
            if (date != null)
            {
                instance.DoctorsAppointment.DoctorAppointmentsDate = date;
                instance.DoctorsAppointmentss = _context.DoctorsAppointments.Where(d => d.DoctorAppointmentsExistedFlag && d.DoctorsId == id && d.DoctorAppointmentsDate == date && d.CustomerId == null).OrderBy(d => d.DoctorAppointmentsTime).ToList();
            }
            int loggedInUserId;            
            Claim user = User.FindFirst(c => c.Type == ClaimTypes.SerialNumber);
            string userValue = user.Value;
            Int32.TryParse(userValue, out loggedInUserId);
            Customer currentCustomer = _context.Customer.FirstOrDefault(c => c.CustomerExistedFlag && c.UsersId == loggedInUserId);
            instance.Customers = currentCustomer;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Registrator, SimpleUser")]
        public IActionResult CreateDoctorsAppointment(DoctorsViewModel doctorAppointment, int id)
        {
            // Если id не соответствуют, то, значит, такого нет, и возвращется HTTP-ответ с кодом 404 ("Не найдено").
            if (id != doctorAppointment.Doctor.DoctorsId)
            {
                return NotFound();
            }

            // Проверка модели на правильность.
            try
            {
                // Отметка о том, что профиль активный (не удалённый).
                doctorAppointment.DoctorsAppointment.DoctorAppointmentsExistedFlag = true;
                doctorAppointment.DoctorsAppointment.DoctorAppointmentsId = _context.DoctorsAppointments.AsNoTracking().FirstOrDefault(d => d.DoctorAppointmentsExistedFlag && d.DoctorsId == id && d.DoctorAppointmentsDate == doctorAppointment.DoctorsAppointment.DoctorAppointmentsDate && d.DoctorAppointmentsTime == doctorAppointment.DoctorsAppointment.DoctorAppointmentsTime && d.CustomerId == null).DoctorAppointmentsId;
                doctorAppointment.DoctorsAppointment.DoctorsId = id;
                // Занесение сделанных правок в базу данных.
                _context.Update(doctorAppointment.DoctorsAppointment);
                // Сохранение изменений в базе данных.
                _context.SaveChanges();
            }
            // Проверка на конкурирующие запросы к базе данных.
            catch (DbUpdateConcurrencyException)
            {
                // Если такого профиля нет, то возвращется HTTP-ответ с кодом 404 ("Не найдено").
                if ((!InstanceExists(doctorAppointment.DoctorsAppointment.DoctorAppointmentsId)) || (!doctorAppointment.DoctorsAppointment.DoctorAppointmentsExistedFlag))
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

        // Проверка телефона доктора на дупликацию.
        // POST: Doctors/CheckInstanceName
        [HttpPost]
        [Authorize(Roles = "Registrator, SimpleUser")]
        public JsonResult CheckInstanceName(string instanceNameForCheck, int? instanceIdToPass)
        {
            // Флаг проверки.
            bool comparisonResultFlag = _context.Doctors.Any(s => s.DoctorsExistedFlag && s.DoctorsPhoneNumber == instanceNameForCheck);
            // Флаг проверки: на случай, если полученные данные совпадают с самими собой в проверяемом профиле.
            bool theSameProfile = false;
            int instanceIdToCheck = -1;
            if (comparisonResultFlag)
            {
                instanceIdToCheck = _context.Doctors.Where(s => s.DoctorsExistedFlag && s.DoctorsPhoneNumber == instanceNameForCheck).SingleOrDefault().DoctorsId;
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

        // Проверка даты.
        // POST: Doctors/CheckInstanceName
        [HttpPost]
        [Authorize(Roles = "Registrator, SimpleUser")]
        public JsonResult CheckInstanceAppointmentDate(int id, string commonInstanceName)
        {
            DateTime tmpDate = Convert.ToDateTime(commonInstanceName);
            string date = tmpDate.ToString("yyyy-MM-dd");
            // Флаг проверки.
            bool instanceDateToCheckFlag = _context.DoctorsAppointments.Any(s => s.DoctorAppointmentsExistedFlag && s.DoctorsId == id && s.DoctorAppointmentsDate == date && s.CustomerId == null);
            if (instanceDateToCheckFlag)
            {
                return Json(0);
            }
            else
            {
                return Json(1);
            }
        }

        [Authorize(Roles = "Registrator, SimpleUser")]
        private bool InstanceExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorsId == id);
        }
    }
}
