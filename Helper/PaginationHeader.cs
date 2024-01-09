using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPage, int totalCount, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }

        public int ItemsPerPage { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}