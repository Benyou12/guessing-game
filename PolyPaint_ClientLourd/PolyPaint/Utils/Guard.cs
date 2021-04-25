using System;
using System.Text.RegularExpressions;

namespace PolyPaint.Utils
{
    public static class Guard
    {
        public static bool IsNullOrEmpty(string value)
        {
            return (value == null || value == String.Empty);
        }

        public static bool IsValidEmail(this string email)
        {
            string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

            if (Regex.IsMatch(email, expresion))
            {
                return (Regex.Replace(email, expresion, string.Empty).Length == 0);
            }
            return false;
        }

    }
}
