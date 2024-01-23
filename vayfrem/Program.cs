using Avalonia;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace vayfrem
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [STAThread]
        public static void Main(string[] args)
        {
            if(!IsAssociated())
            {
                Associate();
            }

            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

        public static bool IsAssociated()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.vayfrem") != null;
            }
            return false;
        }


        public static void Associate()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vayfrem.exe");
                    string assetsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
                    string iconpath = Path.Combine(assetsFolderPath, "icon.ico");
                    if (!File.Exists(iconpath))
                    {
                        return;
                    }

                    RegistryKey FileReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.vayfrem");
                    RegistryKey AppReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\Applications\\vayfrem.exe");
                    RegistryKey AppAssoc = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.vayfrem");


                    FileReg.CreateSubKey("DefaultIcon").SetValue("", iconpath);
                    FileReg.CreateSubKey("PerceivedType").SetValue("", "Text");

                    if (!File.Exists(exePath))
                    {
                        return;
                    }
                    AppReg.CreateSubKey("shell\\open\\command").SetValue("", "\"" + exePath + "\" %1");
                    AppReg.CreateSubKey("shell\\edit\\command").SetValue("", "\"" + exePath + "\" %1");
                    AppReg.CreateSubKey("DefaultIcon").SetValue("", iconpath);

                    AppAssoc.CreateSubKey("UserChoice").SetValue("Progid", "Applications\\vayfrem.exe");
                    SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
