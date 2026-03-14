using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using VencordAutoPatcher;

internal class Program
{
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private static void Main(string[] args)
    {
        IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
        ShowWindow(h, 0);

        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string vapDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VencordAutoPatcher");
        string configPath = Path.Combine(vapDir, ".firstlaunch");
        string installerPath = Path.Combine(vapDir, "VencordCLI.exe");
        string discordUpdateExe = Path.Combine(userProfile, "AppData", "Local", "Discord", "Update.exe");

        if (args.Length > 0 && args[0] == "-autorun")
        {
            Process updater = null;

            while (updater == null)
            {
                updater = Process.GetProcessesByName("Update").FirstOrDefault();
                Process discord = Process.GetProcessesByName("Discord").FirstOrDefault();

                if (updater == null && discord != null)
                    Environment.Exit(0);

                Thread.Sleep(1000);
            }

            Notify("Discord has been updated, patching will occur.");
            updater.WaitForExit();

            Patcher.PatchDiscord(installerPath);
            Environment.Exit(0);
        }

        if (!File.Exists(configPath))
        {
            using (TaskDialog td = new TaskDialog())
            {
                td.Caption = "Vencord Auto Patcher";
                td.InstructionText = "Do you want to setup Auto Run task?";
                td.Text = "It looks like it's your first time running this tool.";

                td.Icon = TaskDialogStandardIcon.Shield;

                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;

                TaskDialogResult result = td.Show();

                if (result == TaskDialogResult.Yes)
                {
                    AutoRun.AddToStartup();
                    File.WriteAllText(configPath, "");
                }

                Patcher.PatchDiscord(installerPath);
            }
        }
        else
        {
            using (TaskDialog td = new TaskDialog())
            {
                td.Caption = "Vencord Auto Patcher";
                td.InstructionText = "Do you want to remove Auto Run task?";
                td.Text = "This will get rid of Vencord Auto Patcher running every start-up.";

                td.Icon = TaskDialogStandardIcon.Information;

                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;

                TaskDialogResult result = td.Show();

                if (result == TaskDialogResult.Yes)
                {
                    AutoRun.RemoveFromStartup();
                    File.Delete(configPath);
                }
            }
        }
    }

    public static void Notify(string message)
    {
        new ToastContentBuilder() .AddText("Vencord Auto Patcher")
            .AddText(message) 
            .Show(); 
    }
}