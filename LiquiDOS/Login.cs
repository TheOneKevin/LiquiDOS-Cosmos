using Cosmos.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LiquiDOS
{
    public class Login
    {
        private string password = "", user = "", name = ""; public int group = 0;
        private byte[] GetHash(string inputString)
        {
            byte[] result;
            var shaM = new SHA1Managed();
            result = shaM.ComputeHash(ByteConverter.GetUtf8Bytes(inputString, 0, (uint)inputString.Length));
            return result;
        }

        /*private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }*/

        public void createUser(string[] logins, string usern, string passw, int group, string userd)
        {
            string date = Drivers.Time.Month() + Drivers.Time.DayOfMonth() + Drivers.Time.Century() + Drivers.Time.Year() + "";
            string time = Drivers.Time.Hour() + Drivers.Time.Minute() + Drivers.Time.Second() + "";
            //$user:[name]$pswd:#NAL#$date:#NAL#$group:[number]$name:[string]
            string t = "";
            foreach(string s in logins)
            {
                t = string.Concat(t, Environment.NewLine, "$user:" + user + "$pswd:" + passw + "$group:" + group + "$name:" + userd);
            }
            File.WriteAllText(@"0:\users.dat", t);
        }
        
        public void list(string[] logins)
        {
            Console.WriteLine("Users on the computer:");
            foreach(string l in logins)
            {
                parseToken(l);
                Console.WriteLine("Username: " + user + ", Display Name: " + name);
            }
        }
        
        public bool validateLogin(int tries, string login, string input, string usern)
        {
            //$user:[name]$pswd:#NAL#$date:#NAL#$group:[number]$name:[string]
            string[] lines = login.Split('$');
            foreach(string entry in lines)
                parseToken(entry);
            if (input == password && usern == user)
            {
                Kernel.PrintDebug("We are in 1!");
                return true;
            }
            else
            {
                Kernel.PrintDebug("We are in hot water!");
                return false;
            }
        }

        private void parseToken(string entry)
        {
            string[] token = entry.Split(':');
            if (token.Length >= 2)
            {
                switch (token[0])
                {
                    case "user": user = token[1]; Kernel.PrintDebug("User:" + token[1]); break;
                    case "pswd": if (token[1] == "#NAL#") password = ""; else password = token[1]; Kernel.PrintDebug("Pass:" + token[1]); break;
                    case "date": break;
                    case "group": group = getGroup(token[1]);  break;
                    case "name": name = token[1]; break;
                    default: break;
                }
            }
        }

        private int getGroup(string group)
        {
            switch(group)
            {
                case "00": return 0;
                case "01": return 1;
                default: return 1;
            }
        }
    }
}
