using Cosmos.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LiquiDOS
{
    public static class Login
    {
        private static string password = "", user = "", name = ""; public static int group = 0;
        private static byte[] GetHash(string inputString)
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

        public static bool validateLogin(int tries, string login, string input, string usern)
        {
            //$user:[name]$pswd:#NAL#$date:#NAL#$group:[number]$name:[string]
            string[] lines = login.Split('$');
            foreach(string entry in lines)
                parseToken(entry);
            if (password == "#NAL#" && string.IsNullOrWhiteSpace(input))
                return true;
            else if (GetHash(input) == GetHash(password) && usern == user)
                return true;
            else
                return false;
        }

        private static void parseToken(string entry)
        {
            string[] token = entry.Split(':');
            if (token.Length >= 2)
            {
                switch (token[0])
                {
                    case "user": user = token[1]; break;
                    case "pswd": if (token[1] == "#NAL#") password = ""; else password = token[1];  break;
                    case "date": break;
                    case "group": group = getGroup(token[1]);  break;
                    case "name": name = token[1]; break;
                    default: break;
                }
            }
        }

        private static int getGroup(string group)
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
