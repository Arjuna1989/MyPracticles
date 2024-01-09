using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; }
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? _pageSize : value;
        }

        public string CurrentUserName { get; set; }

        public string Gender { get; set; }     

        public int MinAge { get; set; } =18;

        public int MaxAge { get; set; } =100;

        public string OrderBy { get; set; } = "LastActive";

    }
}