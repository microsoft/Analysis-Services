using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace BismNormalizer.IconSetup
{
    class Program
    {
        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        const string _extension = ".bsmn";
        const string _progId = "BismNormalizer.BismNormalizerPackage";

        static void Main(string[] args)
        {
            try
            {
                string exeFullName = System.Reflection.Assembly.GetExecutingAssembly().Location;

                //Copy icon to program files
                string iconFullName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\BISM Normalizer";
                CreateDirIfNecessary(iconFullName);
                iconFullName += "\\Icon";
                CreateDirIfNecessary(iconFullName);
                iconFullName += "\\Package.ico";
                if (!File.Exists(iconFullName))
                {
                    File.Copy(exeFullName.Replace("BismNormalizer.IconSetup.exe", "Resources\\Package.ico"), iconFullName);
                }

                //----------------------
                //Find VS install path and create dos command
                string vsVersion = "14.0";
                if (exeFullName.Contains("VisualStudio"))
                {
                    int endVsChar = exeFullName.LastIndexOf("VisualStudio") + 13;
                    if (exeFullName.Length > endVsChar + 4)
                    {
                        string candidateVsVersion = exeFullName.Substring(endVsChar, 4);
                        decimal vsVersionDecimal;
                        if (decimal.TryParse(candidateVsVersion, out vsVersionDecimal))
                        {
                            vsVersion = candidateVsVersion;
                        }
                    }
                }
                string command = "\"" + (String)Registry.GetValue(String.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\" + vsVersion), "InstallDir", "") + "devenv.exe\" \"%1\"";
                //Console.WriteLine("VS Install Path: " + vsInstallPath);

                RegistryKey fileReg = Registry.ClassesRoot.CreateSubKey(".bsmn");
                fileReg.SetValue("", _progId);
                fileReg.CreateSubKey("DefaultIcon").SetValue("", iconFullName);
                fileReg.CreateSubKey("PerceivedType").SetValue("", "Text");

                RegistryKey appReg = Registry.ClassesRoot.CreateSubKey(_progId);
                appReg.SetValue("", "BISM Normalizer file");
                appReg.CreateSubKey("DefaultIcon").SetValue("", iconFullName);

                RegistryKey shell = appReg.CreateSubKey("Shell");
                shell.CreateSubKey("open").CreateSubKey("command").SetValue("", command);
                shell.CreateSubKey("edit").CreateSubKey("command").SetValue("", command);

                fileReg.Close();
                appReg.Close();
                shell.Close();

                RegistryKey appAssoc = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.bsmn");
                appAssoc.CreateSubKey("UserChoice").SetValue("Progid", _progId, RegistryValueKind.String);
                appAssoc.Close();

                SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);

                Console.WriteLine("Set up of icon complete.  Re-open Visual Studio to see the icon in Solution Explorer.  Can also open .bsmn files from Windows Explorer.");
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception occurred:");
                Console.WriteLine(exc.Message);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        private static void CreateDirIfNecessary(string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
        }
    }
}
