using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HangulClockPrivate
{
    public static class Settings
    {
        public static String SERVICE_ENABLED = "a03cfb0f8a0e48cb848f7ba3287b3ecc";

        public static String USE_WALLPAPER = "5fcd3ecae2624959b756fb902080c03d";
        public static String USE_SOLID = "61407e0777724d04bd6b07004d520880";
        public static String SOLID_COLOR = "28438bb86a804674be332816a441b3c2";

        public static String NAME = "a35a895ade424abda1d1fcfffb819a7";
        public static String THEME = "39a03ec41c5247a7a922ac2b6b2c6f53";

        private static string SECTION = typeof(Settings).Namespace;
        private static string settingsPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\hangulClock.hcp";
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        public static String GetString(String name)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(SECTION, name, "", temp, 255, settingsPath);
            return temp.ToString();
        }
        public static String Get(String name, String defVal)
        {
            return Get(SECTION, name, defVal);
        }
        public static String Get(string _SECTION, String name, String defVal)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(_SECTION, name, "", temp, 255, settingsPath);
            return temp.ToString();
        }
        public static Boolean Get(String name, Boolean defVal)
        {
            return Get(SECTION, name, defVal);
        }
        public static Boolean Get(string _SECTION, String name, Boolean defVal)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(_SECTION, name, "", temp, 255, settingsPath);
            bool retval = false;
            if (bool.TryParse(temp.ToString(), out retval))
            {
                return retval;
            }
            else
            {
                return retval;
            }
        }
        public static int Get(String name, int defVal)
        {
            return Get(SECTION, name, defVal);
        }
        public static int Get(string _SECTION, String name, int defVal)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(SECTION, name, "", temp, 255, settingsPath);
            int retval = 0;
            if (int.TryParse(temp.ToString(), out retval))
            {
                return retval;
            }
            else
            {
                return retval;
            }
        }
        public static void Set(String name, String val)
        {
            Set(SECTION, name, val);
        }
        public static void Set(string _SECTION, String name, String val)
        {
            WritePrivateProfileString(_SECTION, name, val, settingsPath);
        }
        public static void Set(String name, Boolean val)
        {
            Set(SECTION, name, val);
        }
        public static void Set(string _SECTION, String name, Boolean val)
        {
            WritePrivateProfileString(_SECTION, name, val.ToString(), settingsPath);
        }
        public static void Set(String name, int val)
        {
            Set(SECTION, name, val);
        }
        public static void Set(string _SECTION, String name, int val)
        {
            WritePrivateProfileString(SECTION, name, val.ToString(), settingsPath);
        }
    }
}
