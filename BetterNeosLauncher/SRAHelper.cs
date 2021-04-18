using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterNeosLauncher
{
    class SRAHelper
    {
        public static string GetSRAnipalEXE()
        {
            string SRAnipalEXE = String.Empty;
            try
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                if (baseKey != null)
                {
                    RegistryKey softwareKey = baseKey.OpenSubKey("Software");
                    if(softwareKey != null)
                    {
                        RegistryKey viveKey = softwareKey.OpenSubKey("VIVE");
                        if(viveKey != null)
                        {
                            RegistryKey srworksKey = viveKey.OpenSubKey("SRWorks");
                            if(srworksKey != null)
                            {
                                RegistryKey sranipalKey = srworksKey.OpenSubKey("SRanipal");
                                if(sranipalKey != null)
                                {
                                    Object o = sranipalKey.GetValue("ModuleFileName");
                                    if(o != null)
                                    {
                                        SRAnipalEXE = o.ToString();
                                    }
                                    else { throw new Exception(); }
                                }
                                else { throw new Exception(); }
                            }
                            else { throw new Exception(); }
                        }
                        else { throw new Exception(); }
                    }
                    else { throw new Exception(); }
                }
                else { throw new Exception(); }
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to find SteamPath!");
                Console.WriteLine(e);
            }

            return SRAnipalEXE;
        }
    }
}
