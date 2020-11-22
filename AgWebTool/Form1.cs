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
using AgWebTool.Properties;

namespace AgWebTool
{
    public partial class Form1 : Form
    {
        public bool nSettings { get; set; }
        public string strUrl { get; set; }
        public float nMaxVideoLength { get; set; }
        public int nStopPosition { get; set; }
        public Point vlcOriginalPoint { get; set; }
        public Size vlcOriginalSize { get; set; }
        public Size formOriginalSize { get; set; }
        public bool bPause { get; set; }
        public bool bPauseCheck { get; set; }
        public bool bTrackHold { get; set; }
        public bool bVolumeHold { get; set; }
        public bool bFullScreen { get; set; }
        public bool bFullScreenGUI { get; set; }
        public bool bFSGUIOpened { get; set; }
        public bool bFSGUIOpenOnMouseDown { get; set; }
        public bool bOpenWhenMouseMoved { get; set; }
        public int nCurrentItem { get; set; }
        public int nLastItem { get; set; }
        public int nVolume { get; set; }
        public bool bSelectedIndex { get; set; }
        public bool bOpenConsole { get; set; }
        public int nPage { get; set; }
        public int nSelected { get; set; }
        public string strTheme { get; set; }
        public string strPath { get; set; }
        public bool FormMaximized { get; set; }
        public string historyaddr { get; set; }
        public bool bLoadDataOnClick { get; set; }
        
        private bool bLoaded { get; set; }

        float baseW = 0;
        float baseH = 0;

        public List<JsonDataSources> jsondeserializedsources;
        public List<JsonDataTracks> jsondeserializedtracks;

        public List<SearchDataClass> SearchDataClassList = new List<SearchDataClass>();

        public Screen[] screens = Screen.AllScreens;

        private Form2 FullScreenGUI;
        private Form3 MatickaRus;
        private Form4 Link;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public void OpenConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_SHOW);
        }
        public void CloseConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }

        public void OpenFSGUI()
        {
            if (!bFSGUIOpened)
            {
                bFSGUIOpened = true;
                FullScreenGUI = new Form2(this);
                FullScreenGUI.StartPosition = FormStartPosition.Manual;
                FullScreenGUI.Location = new Point(this.Location.X + (this.Width / 2) - FullScreenGUI.Width / 2, this.Location.Y + (this.Height / 2) - FullScreenGUI.Height / 2);
                FullScreenGUI.Show(this);
            }
        }

        public void CloseFSGUI()
        {
            if (FullScreenGUI != null)
            {
                bFSGUIOpened = false;
                FullScreenGUI.Close();
                FullScreenGUI.Dispose();
            }
        }


        public class SearchDataClass
        {
            public string file { get; set; }
            public string fileimg { get; set; }
            public string label { get; set; }
            public string length { get; set; }
            public string size { get; set; }
            public bool quality { get; set; }
            public int likes { get; set; }
        }

        public class JsonDataSources
        {
            public string file { get; set; }
            public string label { get; set; }
        }

        public class JsonDataTracks
        {
            public string file { get; set; }
            public string def { get; set; }
            public string label { get; set; }
            public string kind { get; set; }
        }

        public int a = 0;
        public int c = 0;
        public delegate void UpdateControlsDelegate();

        public void SelectTheme(int nTheme, bool bCustomImage)
        {
            switch (nTheme)
            {
                case 0:
                    textBox1.BackColor = Color.FromArgb(255, 66, 88);
                    panel1.BackColor = Color.FromArgb(255, 66, 88);
                    button10.BackColor = Color.FromArgb(255, 66, 88);
                    button1.BackColor = Color.FromArgb(255, 66, 88);
                    button2.BackColor = Color.FromArgb(255, 66, 88);
                    panel2.BackColor = Color.FromArgb(35, 20, 70);
                    textBox2.BackColor = Color.FromArgb(35, 20, 70);
                    panel3.BackColor = Color.FromArgb(35, 20, 70);
                    panel4.BackColor = Color.FromArgb(35, 20, 70);
                    panel7.BackColor = Color.FromArgb(35, 20, 70);
                    pictureBox1.BackColor = Color.FromArgb(150, 50, 70);
                    if (bCustomImage)
                    {
                        vlcControl1.BackgroundImage = Image.FromFile(strPath);
                    }
                    else
                    {
                        vlcControl1.BackgroundImage = (Image)Resources.ResourceManager.GetObject("red");
                    }
                    vlcControl1.BackColor = Color.Black;
                    trackBar2.BackColor = Color.FromArgb(35, 20, 70);
                    trackBar1.BackColor = Color.FromArgb(35, 20, 70);
                    panel5.BackColor = Color.FromArgb(255, 66, 88);
                    button7.BackColor = Color.FromArgb(255, 66, 88);
                    button8.BackColor = Color.FromArgb(255, 66, 88);
                    label15.BackColor = Color.FromArgb(255, 66, 88);
                    button3.BackColor = Color.FromArgb(255, 66, 88);
                    button4.BackColor = Color.FromArgb(255, 66, 88);
                    button9.BackColor = Color.FromArgb(255, 66, 88);
                    button6.BackColor = Color.FromArgb(255, 66, 88);
                    button5.BackColor = Color.FromArgb(255, 66, 88);
                    button11.BackColor = Color.FromArgb(255, 66, 88);
                    button12.BackColor = Color.FromArgb(255, 66, 88);
                    break;
                case 1:
                    textBox1.BackColor = Color.Teal;
                    panel1.BackColor = Color.Teal;
                    button10.BackColor = Color.Teal;
                    button1.BackColor = Color.Teal;
                    button2.BackColor = Color.Teal;
                    panel2.BackColor = Color.FromArgb(15, 40, 50);
                    textBox2.BackColor = Color.FromArgb(15, 40, 50);
                    panel3.BackColor = Color.FromArgb(15, 40, 50);
                    panel4.BackColor = Color.FromArgb(15, 40, 50);
                    panel7.BackColor = Color.FromArgb(15, 40, 50);
                    pictureBox1.BackColor = Color.FromArgb(32, 70, 90);
                    if (bCustomImage)
                    {
                        vlcControl1.BackgroundImage = Image.FromFile(strPath);
                    }
                    else
                    {
                        vlcControl1.BackgroundImage = (Image)Resources.ResourceManager.GetObject("green");
                    }
                    vlcControl1.BackColor = Color.Black;
                    trackBar2.BackColor = Color.FromArgb(15, 40, 50);
                    trackBar1.BackColor = Color.FromArgb(15, 40, 50);
                    panel5.BackColor = Color.Teal;
                    button7.BackColor = Color.Teal;
                    button8.BackColor = Color.Teal;
                    label15.BackColor = Color.Teal;
                    button3.BackColor = Color.Teal;
                    button4.BackColor = Color.Teal;
                    button9.BackColor = Color.Teal;
                    button6.BackColor = Color.Teal;
                    button5.BackColor = Color.Teal;
                    button11.BackColor = Color.Teal;
                    button12.BackColor = Color.Teal;
                    break;
                default:
                    break;
            }
        }


        public Form1(string theme)
        {
            this.strTheme = theme;

            InitializeComponent();
            InitializeSettings();

            using (System.IO.StreamReader file = new System.IO.StreamReader(@".\version.txt", false))
            {
                label21.Text = "Agnares - v. " + file.ReadLine();
            }

            strPath = "";
            historyaddr = "";
            bOpenConsole = false;
            KeyPreview = true;
            bPause = false;
            bPauseCheck = false;
            bTrackHold = false;
            bVolumeHold = false;
            bFullScreen = false;
            bFullScreenGUI = false;
            bOpenWhenMouseMoved = false;
            bFSGUIOpened = false;
            bFSGUIOpenOnMouseDown = false;
            vlcControl1.PositionChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs>(this.vlcControl1_PositionChanged);
            vlcControl1.Playing += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs>(SetProgresMax);
            vlcOriginalPoint = vlcControl1.Location;
            vlcOriginalSize = new Size(vlcControl1.Width, vlcControl1.Height);
            formOriginalSize = new Size(this.Width, this.Height);
            bSelectedIndex = false;
            FormMaximized = false;
            trackBar1.Value = 50;
            nVolume = trackBar1.Value;
            label16.Text = trackBar1.Value.ToString();
            vlcControl1.Audio.Volume = trackBar1.Value;
            for (int i = 0; i < screens.Length; i++)
            {
                listBox3.Items.Add(screens[i].DeviceName);
            }
            nCurrentItem = 1;
            nPage = nCurrentItem;
            nSelected = 0;
            nLastItem = 0;
            SetFeatureToAllControls(this.Controls);

            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox2.DrawMode = DrawMode.OwnerDrawFixed;
            listBox3.DrawMode = DrawMode.OwnerDrawFixed;
            listBox4.DrawMode = DrawMode.OwnerDrawFixed;

            baseW = (float)this.Width;
            baseH = (float)this.Height;

            switch (strTheme)
            {
                case "None":
                    break;
                case "Red":
                    SelectTheme(0, false);
                    break;
                case "Green":
                    SelectTheme(1, false);
                    break;
                default:
                    if (!strTheme.Contains("Custom"))
                    {
                        MessageBox.Show(string.Format("Motiv {0} nenalezen, program se teď uzavře!", strTheme));
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\theme.txt", false))
                        {
                            file.WriteLine("None");
                        }
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        try
                        {
                            int nStartCmd = strTheme.IndexOf("<PATH>");
                            int nEndCmd = strTheme.LastIndexOf("</PATH>");

                            strPath = strTheme.Substring(nStartCmd + 6, nEndCmd - 13);

                            int nStartTheme = strTheme.IndexOf("<THEME>");
                            int nEndTheme = strTheme.LastIndexOf("</THEME>");

                            string strThemeSelect = strTheme.Substring(nStartTheme + 7, (strTheme.Length - nStartTheme) - (strTheme.Length - nEndTheme) - 7);

                            if (strThemeSelect == "Red")
                                SelectTheme(0, true);
                            else if (strThemeSelect == "Green")
                                SelectTheme(1, true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                    break;
            }
        }

        public string getSettingsOpt(bool bOpt)
        {
            return bOpt ? "true" : "false";
        }

        public void RewriteSettings()
        {
            byte[] settingsData = new UTF8Encoding(true).GetBytes
            (
                "checkbox1:"   + getSettingsOpt(checkBox2.Checked) + 
                "\ncheckbox2:" + getSettingsOpt(checkBox3.Checked) + 
                "\ncheckbox3:" + getSettingsOpt(bLoadDataOnClick)  +
                "\0"
            );

            using (FileStream fs = File.OpenWrite(@".\settings.txt"))
            {
                fs.Write(settingsData, 0, settingsData.Length);
            }
        }

        private void InitializeSettings()
        {
            string[] strOpt = File.ReadAllLines(@".\settings.txt");
            bool[] bOptions = new bool[strOpt.Length];
            for (int i = 0; i < strOpt.Length; i++)
            {
                int nSeparatorIdx = strOpt[i].LastIndexOf(":") + 1;
                strOpt[i] = strOpt[i].Substring(nSeparatorIdx, strOpt[i].Length - nSeparatorIdx).ToLower();
                bOptions[i] = Boolean.Parse(strOpt[i]);
            }

            if(bOptions[0] && !bOptions[1])
                checkBox2.Checked = bOptions[0];
            else if(bOptions[1] && !bOptions[0])
                checkBox3.Checked = bOptions[1];

            bLoadDataOnClick = bOptions[2];

            bLoaded = true;
        }

        public bool GetLoadOnDataStatus()
        {
            return bLoadDataOnClick;
        }

        private void SetControlEventFocus(object sender, EventArgs e)
        {
            var ctrl = (Control)sender;
            if (!ctrl.Name.Contains("listBox") && !ctrl.Name.Contains("textBox"))
            {
                this.ActiveControl = label1;
            }
        }

        private void SetFeatureToAllControls(Control.ControlCollection cc)
        {
            if (cc != null)
            {
                foreach (Control control in cc)
                {
                    control.Enter += new System.EventHandler(SetControlEventFocus);
                }
            }
        }

        private void currentTrackTime()
        {
            int b = (int)vlcControl1.VlcMediaPlayer.Time / 1000;
            int d = b / 60;
            b = b - d * 60;
            string timestamp = d + ":" + b + "/" + c + ":" + a; //min : sec / 
            label5.Text = timestamp;
            if (!bTrackHold)
            {
                trackBar2.Value = (int)(vlcControl1.Position * nMaxVideoLength);
                label16.Text = nVolume.ToString();
            }

            // vlcControl1.Audio.Volume = nVolume; 
            // thread lock volume

            if (FullScreenGUI != null)
            {
                foreach (Control control in FullScreenGUI.Controls)
                {
                    if (control.Name == "panel1")
                    {
                        foreach (Control cc in control.Controls)
                        {
                            if (cc.Name == "label1")
                            {
                                cc.Text = nVolume.ToString();
                            }
                            if (cc.Name == "label5")
                            {
                                cc.Text = timestamp;
                            }
                            if (cc.Name == "trackBar2")
                            {
                                ((TrackBar)cc).Value = trackBar2.Value;
                            }
                        }
                    }
                }
            }
        }

        public void InvokeUpdateControls()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new UpdateControlsDelegate(currentTrackTime));
                }
                else
                {
                    currentTrackTime();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        private void vlcControl1_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            InvokeUpdateControls();
        }

        private void SetProgresMax(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            Invoke(new Action(() =>
            {
                trackBar2.Value = trackBar2.Minimum;
                var vlc = (VlcControl)sender;
                trackBar2.Maximum = (int)vlc.Length / 1000;
                a = (int)vlc.Length / 1000;
                c = a / 60;
                a = a % 60;
                nMaxVideoLength = vlc.Length / 1000;
            }));
        }

        private void VLCMain_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            e.VlcLibDirectory = new DirectoryInfo(@".\VLC");
        }

        public bool ReadURL()
        {
            try
            {
                Console.Clear();

                Uri address = new Uri(strUrl);

                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                using (WebClient webClient = new WebClient())
                {
                    var stream = webClient.OpenRead(address);
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        Console.WriteLine("URL Byla uspesne nactena!");
                        var page = sr.ReadToEnd();

                        var html = new HtmlAgilityPack.HtmlDocument();
                        html.LoadHtml(page);
                        html.OptionFixNestedTags = true;

                        var htmlBody = html.DocumentNode.SelectSingleNode("//section//div//div//meta[@itemprop='contentUrl']");
                        var str = System.Text.RegularExpressions.Regex.Replace(htmlBody.GetAttributeValue("content", null), "&amp;", "&");

                        string resultdata = "";

                        if (str[0] == '?')
                        {
                            htmlBody = html.DocumentNode.SelectSingleNode("(//script)[9]");
                            string data = htmlBody.OuterHtml;

                            int endidx = data.IndexOf("redirectLink");

                            resultdata = data.Substring(8, endidx - 12);

                            var engine = new Jurassic.ScriptEngine();
                            var resultsources = engine.Evaluate("(function() { " + resultdata + " return sources; })()");
                            var jsonobjsources = JSONObject.Stringify(engine, resultsources);
                            jsondeserializedsources = JsonConvert.DeserializeObject<List<JsonDataSources>>(jsonobjsources);

                            listBox1.Items.Clear();
                            for (int i = 0; i < jsondeserializedsources.Count; i++)
                            {
                                listBox1.Items.Add(jsondeserializedsources[i].label);
                            }

                            listBox2.Items.Clear();
                            var resulttracks = engine.Evaluate("(function() { " + resultdata + " return tracks; })()");
                            var jsonobjtracks = JSONObject.Stringify(engine, resulttracks);
                            jsondeserializedtracks = JsonConvert.DeserializeObject<List<JsonDataTracks>>(jsonobjtracks);

                            for (int i = 0; i < jsondeserializedtracks.Count; i++)
                            {
                                listBox2.Items.Add(jsondeserializedtracks[i].label);
                            }
                        }
                        else
                        {
                            JsonDataSources data = new JsonDataSources();
                            data.file = str;
                            data.label = "720p";

                            List<JsonDataSources> tmp = new List<JsonDataSources>();
                            tmp.Add(data);

                            jsondeserializedsources = tmp;

                            listBox1.Items.Clear();
                            listBox2.Items.Clear();
                            for (int i = 0; i < jsondeserializedsources.Count; i++)
                            {
                                listBox1.Items.Add(jsondeserializedsources[i].label);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }

            return true;
        }

        public int GetMaxWidthLB(ListBox listbox)
        {
            int tmpWidth = 0;
            for (int i = 0; i < listbox.Items.Count; i++)
            {
                if (tmpWidth < TextRenderer.MeasureText(listbox.Items[i].ToString(), listbox.Font).Width)
                    tmpWidth = TextRenderer.MeasureText(listbox.Items[i].ToString(), listbox.Font).Width;
            }
            return tmpWidth;
        }

        public void Search(string searchdata, bool bShift, bool bHistory, int nIdx)
        {
            try
            {
                Console.Clear();
                Uri address;
                if (bShift)
                {
                    address = new Uri("https://prehraj.to/hledej/" + searchdata + "?vp-page=" + nIdx);
                    historyaddr = address.ToString();
                }
                else if (bHistory)
                {
                    address = new Uri(historyaddr);
                }
                else
                {
                    address = new Uri("https://prehraj.to/hledej/" + searchdata);
                    historyaddr = address.ToString();
                }

                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                SearchDataClassList.Clear();
                listBox4.Items.Clear();

                using (WebClient webClient = new WebClient())
                {
                    var stream = webClient.OpenRead(address);
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        var page = sr.ReadToEnd();

                        var html = new HtmlAgilityPack.HtmlDocument();
                        html.LoadHtml(page);
                        html.OptionFixNestedTags = true;

                        var Total = html.DocumentNode.SelectNodes("//div[@class='columns lg-4']/div");
                        var Links = html.DocumentNode.SelectNodes("//div[@class='columns lg-4']//div//a");
                        var Titles = html.DocumentNode.SelectNodes("//div[@class='columns lg-4']//div//a//h2");
                        var Likes = html.DocumentNode.SelectNodes("//a//div[@class='video-item-likes']");

                        if (!bShift)
                        {
                            label15.Text = "0/0";
                            nLastItem = 0;
                            nCurrentItem = 1;
                            if (html.DocumentNode.SelectNodes("//div[@class='pagination']//span[@class='pagination-pages']") != null)
                            {
                                var pagination = html.DocumentNode.SelectNodes("//div[@class='pagination']//span[@class='pagination-pages']//a");
                                var lastitem = pagination[pagination.Count - 1].InnerText;
                                label15.Text = "1/" + lastitem;
                                nLastItem = Int32.Parse(lastitem);
                            }
                        }

                        if (Total != null)
                        {
                            for (int i = 0; i < Total.Count; i++)
                            {
                                SearchDataClass DataClass = new SearchDataClass();
                                DataClass.label = Titles[i].InnerText;
                                DataClass.file = Links[i].Attributes["href"].Value;
                                if (Links[i].SelectSingleNode(".//div[@class='video-item-hd']") != null)
                                    DataClass.quality = true;
                                else
                                    DataClass.quality = false;

                                if (Links[i].SelectSingleNode(".//div[@class='video-item-likes']") != null)
                                    DataClass.likes = Int32.Parse(Links[i].SelectSingleNode(".//div[@class='video-item-likes']").InnerText);
                                else
                                    DataClass.likes = 0;

                                if (Links[i].SelectSingleNode(".//div[@class='video-item-info']//strong[@class='video-item-info-time']") != null)
                                    DataClass.length = Links[i].SelectSingleNode(".//div[@class='video-item-info']//strong[@class='video-item-info-time']").InnerText;
                                else
                                    DataClass.length = "0";

                                if (Links[i].SelectSingleNode(".//div[@class='video-item-info']//strong[@class='video-item-info-size']") != null)
                                    DataClass.size = Links[i].SelectSingleNode(".//div[@class='video-item-info']//strong[@class='video-item-info-size']").InnerText;
                                else
                                    DataClass.size = "0";

                                if (Links[i].SelectSingleNode(".//div//img[@class='thumb thumb1']").Attributes["src"].Value != null)
                                    DataClass.fileimg = Links[i].SelectSingleNode(".//div//img[@class='thumb thumb1']").Attributes["src"].Value;
                                else
                                    DataClass.fileimg = "";

                                SearchDataClassList.Add(DataClass);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Nenalezen žádný výsledek!");
                        }
                    }
                }

                for (int i = 0; i < SearchDataClassList.Count; i++)
                {
                    listBox4.Items.Add("[" + i + "] " + SearchDataClassList[i].label);
                }

                listBox4.HorizontalExtent = GetMaxWidthLB(listBox4);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
                cert.Subject,
                error.ToString());

            return false;
        }

        private void SearchMovie()
        {
            if (textBox1.TextLength > 0)
            {
                Search(textBox1.Text, false, false, 0);
            }
            else
            {
                MessageBox.Show("Nic nehledáš negre");
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value >= trackBar1.Minimum && trackBar1.Value <= trackBar1.Maximum)
            {
                nVolume = trackBar1.Value;
            }
        }

        private void vlcMoviePlay(string url, string[] opts)
        {
            if (!vlcControl1.IsPlaying)
            {
                vlcControl1.SetMedia(url, opts);
                vlcControl1.Play();
            }
        }

        private void PlayVideo(bool bOpt)
        {
            vlcControl1.Video.IsMouseInputEnabled = false;
            vlcControl1.Video.IsKeyInputEnabled = false;
            if (listBox1.Items.Count > 0)
            {
                try
                {
                    nSettings = bOpt;
                    if (bOpt)
                    {
                        string url = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].file;
                        string track = jsondeserializedtracks[listBox2.SelectedIndex >= 0 ? listBox2.SelectedIndex : 0].file;
                        string urllabel = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].label;
                        string tracklabel = jsondeserializedtracks[listBox2.SelectedIndex >= 0 ? listBox2.SelectedIndex : 0].label;
                        var client = new WebClient();
                        client.DownloadFile(track, "subtitle.vtt");
                        string[] opts = { @"sub-file=subtitle.vtt" };
                        vlcMoviePlay(url, opts);
                        // vlcControl1.SetMedia(url, opts);
                        // vlcControl1.Play();
                        // media play thread lock
                        if (!bPauseCheck)
                        {
                            if (tracklabel.Contains("ru"))
                            {
                                MatickaRus = new Form3();
                                MatickaRus.Show();
                            }
                            MessageBox.Show("Spuštěno " + urllabel + " s " + tracklabel);
                        }
                        Console.WriteLine("URL: " + url);
                    }
                    else
                    {
                        string url = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].file;
                        string urllabel = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].label;
                        vlcMoviePlay(url, null);
                        // vlcControl1.SetMedia(url);
                        // vlcControl1.Play();
                        // media play thread lock
                        if (bPauseCheck)
                        {
                            vlcControl1.Position = nStopPosition;
                        }
                        else
                        {
                            MessageBox.Show("Spuštěno " + urllabel);
                        }
                        Console.WriteLine("URL: " + url);
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            vlcControl1.Stop();
            if (listBox1.SelectedIndex >= 0 && listBox2.SelectedIndex >= 0)
            {
                PlayVideo(true);
            }
            else
            {
                PlayVideo(false);
            }
        }
        private void ShowFullScreenGUI()
        {
            bFullScreenGUI = !bFullScreenGUI;
            if (bFullScreenGUI)
            {
                OpenFSGUI();
                this.Focus();
            }
            else
            {
                CloseFSGUI();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CallFullScreen();
        }

        public void PauseVideo()
        {
            vlcControl1.ResetMedia();
            bPause = !bPause;
            vlcControl1.SetPause(bPause);
            if (FullScreenGUI != null)
            {
                foreach (Control control in FullScreenGUI.Controls)
                {
                    if (control.Name == "button1")
                    {
                        if (bPause)
                            control.Text = "⏯️";
                        else
                            control.Text = "⏸";
                    }
                }
            }
            if (bPause)
            {
                button4.Text = "⏯️";
                nStopPosition = trackBar2.Value;
            }
            else
            {
                int mins = nStopPosition / 60;
                int secs = nStopPosition % 60;
                Console.WriteLine("Pause position restored at: " + mins + ":" + secs);
                button4.Text = "⏸";
                bPauseCheck = true;
                PlayVideo(nSettings);
                bPauseCheck = false;

                float pos = nStopPosition / nMaxVideoLength;
                vlcControl1.Position = pos;
            }
        }

        public void CallFullScreen()
        {
            bFullScreen = !bFullScreen;
            if (bFullScreen)
            {
                Rectangle resolution;
                if (listBox3.SelectedIndex >= 0)
                {
                    resolution = screens[listBox3.SelectedIndex].Bounds;

                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                    TopMost = true;

                    vlcControl1.Location = new Point(0, 0);

                    vlcControl1.BringToFront();
                    vlcControl1.Width = resolution.Width;
                    vlcControl1.Height = resolution.Height;
                    Console.WriteLine(screens[listBox3.SelectedIndex].ToString());
                }
                else
                {
                    resolution = screens[0].Bounds;

                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                    TopMost = true;

                    vlcControl1.Location = new Point(0, 0);

                    vlcControl1.BringToFront();
                    vlcControl1.Width = resolution.Width;
                    vlcControl1.Height = resolution.Height;
                    Console.WriteLine(screens[0].ToString());
                }
            }
            else
            {
                if (FormMaximized)
                {
                    FormMaximized = false;
                    if (!FormMaximized)
                    {
                        button1.Text = "🗖︎";
                        this.WindowState = FormWindowState.Normal;
                        this.Scale(new SizeF(1.0f / (this.Width / baseW), 1.0f / (this.Height / baseH)));
                    }
                }

                if (bFullScreenGUI)
                    ShowFullScreenGUI();

                if (vlcControl1.Audio.Volume != -1)
                    trackBar1.Value = vlcControl1.Audio.Volume;

                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Normal;
                TopMost = false;

                vlcControl1.Location = vlcOriginalPoint;

                vlcControl1.Width = vlcOriginalSize.Width;
                vlcControl1.Height = vlcOriginalSize.Height;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Modifiers == Keys.Control)
            {
                CallFullScreen();
            }
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (FormMaximized)
                    {
                        FormMaximized = false;
                        if (!FormMaximized)
                        {
                            button1.Text = "🗖︎";
                            this.WindowState = FormWindowState.Normal;
                            this.Scale(new SizeF(1.0f / (this.Width / baseW), 1.0f / (this.Height / baseH)));
                        }
                    }
                    bFullScreen = false;
                    if (bFSGUIOpened)
                        ShowFullScreenGUI();

                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Normal;
                    TopMost = false;

                    vlcControl1.Location = vlcOriginalPoint;

                    vlcControl1.Width = vlcOriginalSize.Width;
                    vlcControl1.Height = vlcOriginalSize.Height;
                    break;
                case Keys.F1:
                    if (bFullScreen)
                    {
                        ShowFullScreenGUI();
                    }
                    break;
                case Keys.Enter:
                    VideoPlayingPause();
                    SearchMovie();
                    break;
                case Keys.Space:
                    if (!textBox1.Focused)
                    {
                        label1.Focus();
                        PauseVideo();
                    }
                    break;
                default:
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Na celou obrazovku - [CTRL + F]\nPauza - [CTRL + SPACE]\nFullscreen GUI - [F1]\nOpustit celou obrazovku - [ESC]");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PauseVideo();
        }

        private void trackBar2_MouseDown(object sender, MouseEventArgs e)
        {
            bTrackHold = true;
        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            bTrackHold = false;
        }

        public void SetLB4SelectedIDX(int nIdx)
        {
            listBox4.SelectedIndex = nIdx;
        }

        public List<int> GetBestQualityIdxArray(bool bOption)
        {
            List<int> ResultArray = new List<int>();
            if (bOption)
            {
                int TemporaryLength = 0;

                // get best quality from searchdataclasslist
                for (int i = 0; i < SearchDataClassList.Count; i++)
                {
                    SearchDataClass list = SearchDataClassList[i];
                    const int maxLen = 3;
                    bool bMegaBytes = false;
                    if (list.size.Contains("MB"))
                    {
                        bMegaBytes = true;
                    }
                    string strSize = System.Text.RegularExpressions.Regex.Replace(list.size.Substring(0, list.size.Length - 4), @"\D+", string.Empty);
                    if (strSize.Length < maxLen)
                    {
                        for (int j = 0; j < (maxLen - strSize.Length); j++)
                        {
                            strSize += "0";
                        }
                    }
                    if (!bMegaBytes)
                    {
                        int nLength = Int32.Parse(strSize);
                        if (TemporaryLength < nLength)
                            TemporaryLength = nLength;
                    }
                }

                // search for duplicates and push to array
                for (int i = 0; i < SearchDataClassList.Count; i++)
                {
                    SearchDataClass list = SearchDataClassList[i];
                    const int maxLen = 3;
                    bool bMegaBytes = false;
                    if (list.size.Contains("MB"))
                    {
                        bMegaBytes = true;
                    }
                    string strSize = System.Text.RegularExpressions.Regex.Replace(list.size.Substring(0, list.size.Length - 4), @"\D+", string.Empty);
                    if (strSize.Length < maxLen)
                    {
                        for (int j = 0; j < (maxLen - strSize.Length); j++)
                        {
                            strSize += "0";
                        }
                    }
                    if ((Int32.Parse(strSize) == TemporaryLength) && !bMegaBytes)
                        ResultArray.Add(i);
                }
            }
            else
            {
                int TemporaryLength = 0;
                for (int i = 0; i < SearchDataClassList.Count; i++)
                {
                    SearchDataClass list = SearchDataClassList[i];
                    if (TemporaryLength < list.likes)
                        TemporaryLength = list.likes;
                }
                for (int i = 0; i < SearchDataClassList.Count; i++)
                {
                    if (SearchDataClassList[i].likes == TemporaryLength)
                        ResultArray.Add(i);
                }
            }    
            return ResultArray;
        }

        public void UpdateLB4(bool bHistory)
        {
            if (listBox4.Items.Count > 0)
            {
                if (listBox4.SelectedItem != null)
                {
                    nPage = nCurrentItem;
                    nSelected = listBox4.SelectedIndex;
                    UpdateInfoPanel(nSelected);
                    bSelectedIndex = true;
                    //label18.Text = "[" + nSelected + "] " + SearchDataClassList[nSelected].label;
                    label18.Visible = false;
                    textBox2.Text = "[" + nSelected + "] " + SearchDataClassList[nSelected].label;
                    strUrl = "https://prehraj.to" + SearchDataClassList[nSelected].file;
                    if (!ReadURL())
                    {
                        MessageBox.Show("Chyba při načítání URL!");
                    }
                    else
                    {
                        if (!bHistory)
                        {
                            string filestr = @".\history.txt";
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filestr, true))
                            {
                                string time = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                file.WriteLine("<ID>" + nSelected + "</ID><ADDRESS>" + historyaddr + "</ADDRESS><NAME>" + SearchDataClassList[nSelected].label + "</NAME><TIMESTAMP>" + time + "</TIMESTAMP>");
                            }
                        }
                    }
                }
            }
        }

        private void listBox4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                VideoPlayingPause();
                UpdateLB4(false);
            }
            else
            {
                bSelectedIndex = false;
                nPage = 0;
            }
        }

        public void UpdateInfoPanel(int nIndex)
        {
            label8.Text = SearchDataClassList[nIndex].length;
            label11.Text = SearchDataClassList[nIndex].size;
            label12.Text = SearchDataClassList[nIndex].likes.ToString();
            label14.Text = SearchDataClassList[nIndex].quality.ToString();

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.ImageLocation = SearchDataClassList[nIndex].fileimg;
        }

        private void listBox4_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = listBox4.PointToClient(Cursor.Position);
            int index = listBox4.IndexFromPoint(point);

            if (index < 0)
            {
                /*label8.Text = "0";
                label11.Text = "0";
                label12.Text = "0";
                label14.Text = "0";

                pictureBox1.ImageLocation = "";

                listBox4.SelectedIndex = -1;*/
                return;
            }

            if (!bSelectedIndex)
            {
                UpdateInfoPanel(index);
            }

            if (!bSelectedIndex)
                listBox4.SelectedIndex = index;
        }

        public void VideoPlayingPause()
        {
            if (vlcControl1.IsPlaying)
            {
                vlcControl1.ResetMedia();
                vlcControl1.SetPause(true);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (nCurrentItem > 1)
            {
                if (textBox1.TextLength > 0)
                {
                    nCurrentItem -= 1;
                    label15.Text = nCurrentItem + "/" + nLastItem;
                    Search(textBox1.Text, true, false, nCurrentItem);
                    if (nCurrentItem == nPage)
                    {
                        listBox4.SelectedIndex = nSelected;
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (nCurrentItem < nLastItem)
            {
                if (textBox1.TextLength > 0)
                {
                    nCurrentItem += 1;
                    label15.Text = nCurrentItem + "/" + nLastItem;
                    Search(textBox1.Text, true, false, nCurrentItem);
                    if (nCurrentItem == nPage)
                    {
                        listBox4.SelectedIndex = nSelected;
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bOpenConsole = !bOpenConsole;
            if (bOpenConsole)
                OpenConsole();
            else
                CloseConsole();
        }

        public bool formExists(Form form)
        {
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Name == form.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateLinkData(Form4 form)
        {
            if (listBox1.Items.Count > 0)
            {
                form.SetUrl(jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].file);
                if (listBox4.SelectedIndex >= 0)
                    form.SetMovName(SearchDataClassList[listBox4.SelectedIndex].label);
                else
                    form.SetMovName("Unknown");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Link = new Form4(this, 0);
            if (!formExists(Link))
            {
                UpdateLinkData(Link);
                Link.Show();
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            int nVal = trackBar2.Value;
            float pos = nVal / nMaxVideoLength;
            vlcControl1.Position = pos;
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            bVolumeHold = true;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            bVolumeHold = false;
        }

        private void vlcControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                CallFullScreen();
        }

        private void vlcControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (bFullScreen)
                    ShowFullScreenGUI();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DrawCustomRectangleLB(ListBox listbox, DrawItemEventArgs e, bool bBestQuality)
        {
            if (e.Index < 0) return;
            // color best quality //                          \/  true = highlight by size, false = highlight by likes
            List<int> QualityArray = GetBestQualityIdxArray(checkBox2.Checked ? true : checkBox3.Checked ? false : false);
            Color lbColor = Color.Aquamarine;

            if (bBestQuality)
            { 
                for (int i = 0; i < QualityArray.Count; i++)
                {
                    if (e.Index == QualityArray[i])
                    {
                        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                            lbColor = Color.Purple;
                        else
                            lbColor = Color.Green;

                        e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, lbColor);
                    }
                }
            }

            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, lbColor);
            }
           
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString(listbox.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        private void listBox4_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawCustomRectangleLB(listBox4, e, true);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawCustomRectangleLB(listBox1, e, false);
        }

        private void listBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawCustomRectangleLB(listBox2, e, false);
        }

        private void listBox3_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawCustomRectangleLB(listBox3, e, false);
        }

        private Point _mouseDown;
        private Point _formLocation;
        private bool _capture;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            _capture = true;
            _mouseDown = e.Location;
            _formLocation = ((Form)TopLevelControl).Location;
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            _capture = false;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (_capture)
            {
                int dx = e.Location.X - _mouseDown.X;
                int dy = e.Location.Y - _mouseDown.Y;
                Point newLocation = new Point(_formLocation.X + dx, _formLocation.Y + dy);
                ((Form)TopLevelControl).Location = newLocation;
                _formLocation = newLocation;
            }
        }

        private void ClearSelected(ListBox listbox, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                listbox.ClearSelected();
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ClearSelected(listBox1, e);
        }

        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            ClearSelected(listBox2, e);
        }

        private void listBox3_MouseDown(object sender, MouseEventArgs e)
        {
            ClearSelected(listBox3, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AutoScaleMode = AutoScaleMode.None;
            FormMaximized = !FormMaximized;

            if (!FormMaximized)
            {
                button1.Text = "🗖︎";
                this.WindowState = FormWindowState.Normal;
                this.Scale(new SizeF(1.0f / (this.Width / baseW), 1.0f / (this.Height / baseH)));
            }
            else
            {
                Rectangle resolution;
                int nWidth = 0;
                int nHeight = 0;
                if (listBox3.SelectedIndex >= 0)
                {
                    resolution = screens[listBox3.SelectedIndex].Bounds;
                    nWidth = resolution.Width;
                    nHeight = resolution.Height;
                }
                else
                {
                    resolution = screens[0].Bounds;
                    nWidth = resolution.Width;
                    nHeight = resolution.Height;
                }
                button1.Text = "🗗︎";
                this.WindowState = FormWindowState.Maximized;
                this.Scale(new SizeF(nWidth / baseW, nHeight / baseH));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Link = new Form4(this, 2);
            if (!formExists(Link))
            {
                UpdateLinkData(Link);
                Link.Show();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listBox4.Items.Count > 0)
            {
                if (listBox4.SelectedItem != null)
                {
                    nPage = nCurrentItem;
                    nSelected = listBox4.SelectedIndex;
                    string filestr = @".\favourite.txt";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(filestr, true))
                    {
                        string time = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        file.WriteLine("<ID>" + nSelected + "</ID><ADDRESS>" + historyaddr + "</ADDRESS><NAME>" + SearchDataClassList[nSelected].label + "</NAME><TIMESTAMP>" + time + "</TIMESTAMP>");
                    }
                }
            }
        }

        private bool CheckBoxesWouldIntersect(CheckBox chb)
        {
            if (chb.Checked && checkBox2.Checked || chb.Checked && checkBox3.Checked)
                return true;
            return false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (bLoaded)
            {
                if (CheckBoxesWouldIntersect(checkBox2))
                    checkBox3.Checked = false;

                RewriteSettings();
                listBox4.Refresh();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (bLoaded)
            {
                if (CheckBoxesWouldIntersect(checkBox3))
                    checkBox2.Checked = false;

                RewriteSettings();
                listBox4.Refresh();
            }
        }

        private void vlcControl1_VideoOutChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerTimeChangedEventArgs e)
        {
            vlcControl1.Audio.Volume = nVolume;
        }

        private void textBox2_DoubleClick(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0)
            {
                string strText = textBox2.Text.Substring(4, textBox2.Text.Length - 4);
                Clipboard.SetText(strText);
                MessageBox.Show("Zkopírováno: " + strText);
            }    
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            _capture = true;
            _mouseDown = e.Location;
            _formLocation = ((Form)TopLevelControl).Location;
        }

        private void textBox2_MouseUp(object sender, MouseEventArgs e)
        {
            _capture = false;
        }

        private void textBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (_capture)
            {
                int dx = e.Location.X - _mouseDown.X;
                int dy = e.Location.Y - _mouseDown.Y;
                Point newLocation = new Point(_formLocation.X + dx, _formLocation.Y + dy);
                ((Form)TopLevelControl).Location = newLocation;
                _formLocation = newLocation;
            }
        }
    }
}