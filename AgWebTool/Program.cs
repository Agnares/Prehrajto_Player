using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using HtmlAgilityPack;
using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json;
using System.Security.Policy;
using Vlc.DotNet.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Timers;

namespace AgWebTool
{
    static class Program
    {
        private static bool CheckVersion()
        {
            Console.Clear();

            Uri address = new Uri("https://github.com/Agnares/Prehrajto_Player/releases/latest");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            using (WebClient webClient = new WebClient())
            {
                var stream = webClient.OpenRead(address);
                using (StreamReader sr = new StreamReader(stream))
                {
                    var page = sr.ReadToEnd();
                    var html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(page);
                    html.OptionFixNestedTags = true;

                    var releaseHeader = html.DocumentNode.SelectSingleNode("//div[@class='release-header']//div//div//a");
                    var releaseAssets = html.DocumentNode.SelectSingleNode("//div[@class='Box Box--condensed mt-3']//div//a");
                    var name = releaseAssets.InnerText.Trim();
                    var link = "https://www.github.com" + releaseAssets.Attributes["href"].Value;

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\version_link.txt", false))
                    {
                        file.WriteLine(link + "&name=" + name + "&newver=" + releaseHeader.InnerText);
                    }

                    /*using (System.IO.StreamReader file = new System.IO.StreamReader(@".\version.txt", false))
                    {
                        string strCurrentVer = file.ReadLine();
                        if (strCurrentVer != releaseHeader.InnerText)
                        { 
                            //MessageBox.Show("Verze není aktuální! Program se teď zavře a začne updatování.\nVerze po updatu [ " + releaseHeader.InnerText + " ]");
                            return false;
                        }
                    }*/
                }
            }
            return true;
        }

        private static bool CheckFile(string strPath)
        {
            return File.Exists(strPath);
        }

        private static void CreateTxtIfNotPresent()
        {
            string[] strFiles =
            {
                @".\settings.txt",
                @".\history.txt",
                @".\favourite.txt",
                @".\theme.txt",
                @".\version.txt",
                @".\version_link.txt"
            };

            byte[] themeNone = new UTF8Encoding(true).GetBytes
            (
                "None"
            );
            byte[] settingsData = new UTF8Encoding(true).GetBytes
            (
                "checkbox1:true\ncheckbox2:false\ncheckbox3:true\0"
            );

            foreach (string strFile in strFiles)
            {
                if (!CheckFile(strFile))
                {
                    using (FileStream fs = File.Create(strFile))
                    {
                        if (strFile == strFiles[3])
                            fs.Write(themeNone, 0, themeNone.Length);

                        if (strFile == strFiles[0])
                            fs.Write(settingsData, 0, settingsData.Length);

                    }
                }
                else
                {
                    byte[] bytesRead = File.ReadAllBytes(strFile);
                    using (FileStream fs = File.OpenWrite(strFile))
                    {
                        if (strFile == strFiles[3] && (bytesRead.Length < themeNone.Length))
                            fs.Write(themeNone, 0, themeNone.Length);
                        if (strFile == strFiles[0] && (bytesRead.Length < settingsData.Length))
                            fs.Write(settingsData, 0, settingsData.Length);
                    }
                }
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public class MultiFormContext : ApplicationContext
        {
            private int openForms;
            public MultiFormContext(params Form[] forms)
            {
                openForms = forms.Length;

                foreach (var form in forms)
                {
                    form.FormClosed += (s, args) =>
                    {
                        //When we have closed the last of the "starting" forms, 
                        //end the program.
                        if (Interlocked.Decrement(ref openForms) == 0)
                            ExitThread();
                    };

                    form.Show();
                }
            }
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;

        [STAThread]
        static void Main()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            if (CheckVersion())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                string strTheme;
                Form5 Theme = new Form5();
                CreateTxtIfNotPresent();
                using (System.IO.StreamReader file = new System.IO.StreamReader(@".\theme.txt", false))
                {
                    strTheme = file.ReadLine();
                    if (strTheme == "None")
                    {
                        Screen screen = Screen.FromControl(Theme);
                        Theme.Location = new Point(screen.Bounds.Width / 2 - Theme.Width / 2, screen.Bounds.Height / 2 - Theme.Height / 2);
                        Theme.ShowDialog();
                        strTheme = Theme.Theme;
                    }
                }
                using (System.IO.StreamWriter fw = new System.IO.StreamWriter(@".\theme.txt", false))
                {
                    fw.WriteLine(strTheme);
                }

                //Application.Run(new MultiFormContext(new Form6(), new Form1(strTheme)));
                Application.Run(new Form1(strTheme));
            }
            else
            {
                System.Diagnostics.Process.Start(@".\AgWebToolPatcher.exe");
            }
        }
    }
}
