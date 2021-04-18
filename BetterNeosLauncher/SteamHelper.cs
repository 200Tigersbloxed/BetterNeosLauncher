using Microsoft.Win32;
using System;
using System.IO;
using Indieteur.VDFAPI;
using Indieteur.SAMAPI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterNeosLauncher
{
    class SteamHelper
    {
        public static string FindSteamPath()
        {
            string strToReturn = String.Empty;
            try
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                if(baseKey != null)
                {
                    RegistryKey softwareKey = baseKey.OpenSubKey("Software");
                    if (softwareKey != null)
                    {
                        RegistryKey valveKey = softwareKey.OpenSubKey("Valve");
                        if(valveKey != null)
                        {
                            RegistryKey steamKey = valveKey.OpenSubKey("Steam");
                            if(steamKey != null)
                            {
                                Object o = steamKey.GetValue("SteamPath");
                                if (o != null)
                                {
                                    strToReturn = o.ToString();
                                }
                                else { throw new Exception(); }
                            }
                        }
                        else { throw new Exception(); }
                    }
                    else { throw new Exception(); }
                }
                else { throw new Exception(); }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find SteamPath!");
                Console.WriteLine(e);
            }

            return strToReturn.Replace('/', char.Parse(@"\"));
        }

        public static string FindLibraryFoldersPath(string SteamInstallPath)
        {
            string thePath = SteamInstallPath + @"/steamapps/libraryfolders.vdf";
            if (!File.Exists(thePath))
            {
                throw new Exception("libraryfolders VDF File not found!");
            }

            return thePath;
        }

        public static VDFData TryParseVDF(string path) { return new VDFData(path); }
        public static SteamAppsManager CreateSteamAppsManager(string pathToSteam) { return new SteamAppsManager(pathToSteam); }

        public static string FindNeosDir(SteamAppsManager sam)
        {
            SteamApp NeosApp = null;
            foreach(SteamApp app in sam.SteamApps)
            {
                if(app.AppID == 740250)
                {
                    NeosApp = app;
                }
            }
            if(NeosApp == null) { throw new Exception("Neos was not found! Is it installed?"); }
            return NeosApp.InstallDir;
        }
    }
}
