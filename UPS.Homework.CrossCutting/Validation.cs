using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UPS.Homework.CrossCutting
{
    public class Validation
    {
        public static bool IsEmailValid(string email)
        {
            var reg = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$", RegexOptions.IgnoreCase);
            if (reg.IsMatch(email))
            {
                return true;
            }

            return false;
        }
    }
}
