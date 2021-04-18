using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterNeosLauncher
{
    class PrettyFormat
    {
        public static string Directory(string dir)
        {
            string strToReturn = dir;
            string[] splitStr = strToReturn.Split(char.Parse(@"\"));
            splitStr[0] = splitStr[0].ToUpper();
            strToReturn = String.Empty;
            int i = 0;
            foreach(string str in splitStr)
            {
                if (i != 0)
                    strToReturn = strToReturn + @"\" + str;
                else
                    strToReturn = strToReturn + str;
                i++;
            }

            return strToReturn;
        }
    }
}
