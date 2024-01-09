using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtension
    {
        public static int CalculateAge(this DateOnly DOB)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DOB.Year;
            if (DOB > today.AddYears(-age)) age--;
            return age;
        }
    }
}