using System;

namespace Api.Extensions
{
    public static class CaculaAge
    {
        public static int CalculateUserAge(this DateTime dob)
        {
            var today = DateTime.Now;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
