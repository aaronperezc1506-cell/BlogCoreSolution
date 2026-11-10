using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogCoreSolution.Models
{
    public class ListaPaginada<T> : List<T>
    {
        public int PageIndex { get; private set; }

        public int TotalPages { get; private set; }

        public string SearchString { get; set; }

        public ListaPaginada(
            List<T> items,
            int count,
            int pageIndex,
            int pageSize,
            string searchString = "")
        {
            PageIndex = pageIndex;

            TotalPages = (int)Math.Ceiling(
                count / (double)pageSize
            );

            SearchString = searchString;

            AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }

        public static ListaPaginada<T> CrearPaginacion(
            IQueryable<T> source,
            int pageIndex,
            int pageSize,
            string searchString = "")
        {
            int count = source.Count();

            List<T> items = source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ListaPaginada<T>(
                items,
                count,
                pageIndex,
                pageSize,
                searchString
            );
        }
    }
}