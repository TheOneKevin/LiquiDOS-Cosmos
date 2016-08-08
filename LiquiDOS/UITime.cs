using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquiDOS
{
    public static class UITime
    {
        public static string strYear(int year)
        {
            string s = "";
            switch(year)
            {
                case 1: s = "January"; break;
                case 2: s = "Feburary"; break;
                case 3: s = "March"; break;
                case 4: s = "April"; break;
                case 5: s = "May"; break;
                case 6: s = "June"; break;
                case 7: s = "July"; break;
                case 8: s = "August"; break;
                case 9: s = "September"; break;
                case 10: s = "October"; break;
                case 11: s = "November"; break;
                case 12: s = "December"; break;
                default: s = "Invalid Year: " + year; break;
            }
            return s;
        }

        public static string dayOfWeek(int dayOfWeek)
        {
            string s = "";
            return s;
        }
    }
}
