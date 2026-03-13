using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using VencordAutoPatcher;

internal class Program
{
    static string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    static string vapDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VencordAutoPatcher");
    static string configPath = Path.Combine(vapDir, ".firstlaunch");
    static string installerPath = Path.Combine(vapDir, "VencordCLI.exe");
    static string discordUpdateExe = Path.Combine(userProfile, "AppData", "Local", "Discord", "Update.exe");
    static void Settings()
    {
        Console.Clear();
        ConsoleUtilities.Log("Available options:");
        ConsoleUtilities.Log("(r): clear firstlaunch flag");
        string option = ConsoleUtilities.Ask();
        if (option == "r") // TODO: make this a switch
            File.Delete(configPath);
        ConsoleUtilities.Log("press any key to continue.");
        Console.ReadKey();
        return;
    }

    private static void Main(string[] args)
    {
        Console.Title = "VencordAutoPatcher";
        Console.SetWindowSize(70, 30);
        Console.SetBufferSize(70, 30);

        if (args.Length < 1)
        {
            if (!File.Exists(configPath))
            {
                Directory.CreateDirectory(vapDir);
                ConsoleUtilities.Log("It looks like it's your first time running VAP");
                ConsoleUtilities.Log("Do you want to setup Auto Start task? (y/n)");
                string choice = ConsoleUtilities.Ask();

                if (choice == "y" || choice == "yes")
                    AutoRun.AddToStartup();

                File.WriteAllText(configPath, "");

                ConsoleUtilities.Log("Press any key to close. Re-launch to open settings");
                Console.ReadKey();
                Environment.Exit(0);
            }

            while (true)
                Settings();
        }


        switch (args[0])
        {
            case "-autorun":
                Console.Clear();

                ConsoleUtilities.Log("Downloading Vencord CLI Installer");
                using (WebClient web = new WebClient())
                    web.DownloadFile("https://github.com/Vencord/Installer/releases/latest/download/VencordInstallerCli.exe", installerPath);

                ConsoleUtilities.Log("Successfully downloaded VencordInstallerCli.exe");

                ConsoleUtilities.Log("Attempting to kill Discord process for patching.");
                foreach (var process in Process.GetProcessesByName("Discord"))
                    try { process.Kill(); } catch { }

                ConsoleUtilities.Log("Attempting to patch!");
                Process patcher = new Process();
                patcher.StartInfo.FileName = installerPath;
                patcher.StartInfo.Arguments = "-install -branch auto";
                patcher.Start();
                patcher.WaitForExit();

                ConsoleUtilities.Log("Patching completed.");

                if (File.Exists(discordUpdateExe))
                {
                    ConsoleUtilities.Log("Relaunching Discord.");
                    Process.Start(discordUpdateExe);
                }
                else
                {
                    ConsoleUtilities.Log("Update.exe not found, cannot restart Discord automatically!!!");
                }
                break;
        }
    }
}