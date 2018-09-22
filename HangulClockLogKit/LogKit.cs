/**
 * Project : Hangul Clock v3 (HookKit)
 * Author : Lee Dora (leeshoon1344@gmail.com)
 * License : GPLv3
 * Description : LogKit for Hangul Clock
 */

using System;
using System.IO;

namespace HangulClockLogKit
{
    public class LogKit
    {
        private static String WARN = "[WARNING] {0} : {1}";
        private static String INFO = "[INFO] {0} : {1}";
        private static String ERROR = "[ERROR] {0} : {1}";

        private enum Type
        {
            WARN,
            INFO,
            ERROR
        }

        public static void Warn(string logMessage)
        {
            Log(Type.WARN, logMessage);
        }

        public static void Error(string logMessage)
        {
            Log(Type.ERROR, logMessage);
        }

        public static void Info(string logMessage)
        {
            Log(Type.INFO, logMessage);
        }

        private static void Log(Type t, string message)
        {
            string logPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hangul Clock";
            DateTime today = DateTime.Now;

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            try
            {
                using (StreamWriter w = File.AppendText(logPath + "\\HangulClock_" + today.ToString("yyyy-MM-dd") + ".log"))
                {
                    if (t == Type.ERROR)
                    {
                        LogInternal(String.Format(ERROR, today.Hour + ":" + today.Minute + ":" + today.Second, message), w);
                    }
                    else if(t == Type.INFO)
                    {
                        LogInternal(String.Format(INFO, today.Hour + ":" + today.Minute + ":" + today.Second, message), w);
                    }
                    else
                    {
                        LogInternal(String.Format(WARN, today.Hour + ":" + today.Minute + ":" + today.Second, message), w);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private static void LogInternal(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write(logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
