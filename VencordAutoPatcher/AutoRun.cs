using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VencordAutoPatcher
{
    internal class AutoRun
    {
        public static void AddToStartup()
        {
            string vapDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VencordAutoPatcher");
            string newPath = Path.Combine(vapDir, "VencordAutoPatcher.exe");

            Console.Clear();
            File.Copy(Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe"), newPath, true);
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.SetValue("Vencord Auto Patcher", $"{newPath} -autorun");
            key.Close();
            ConsoleUtilities.Log("Successfully added VAP to Startup");
        }
    }
}
    