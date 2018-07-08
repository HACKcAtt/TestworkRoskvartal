using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestWork.Models
{
    /// <summary>
    /// Класс пагинации вывода данных из контроллера в представление.
    /// </summary>
    public class PaginatedList<T> : List<T>
    {
        // Индекс текущей страницы при пагинации.
        public int PageIndex { get; private set; }
        // Всего суммарно страниц для пагинации.
        public int TotalPages { get; private set; }
        // Строка поискового запроса для поиска профиля.
        public string searchQuery { get; set; }
        // Непосредственная реализация пагинации (конструктор класса).
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }
        // Проверка на наличие предыдущих страниц (или же текущая страница - первая).
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }
        // Проверка на наличие следующих страниц (или же текущая страница - крайняя).
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
        // Переопределение метода CreateAsync для вывода данных из контроллера в виде для пагинации.
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}