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
using AxWMPLib;
using System.Security.Policy;
using Vlc.DotNet.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Timers;

namespace AgWebTool
{
    public partial class Form1 : Form
    {
        public int nSettings { get; set; }
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

        public List<JsonDataSources> jsondeserializedsources;
        public List<JsonDataTracks> jsondeserializedtracks;

        public List<SearchDataClass> SearchDataClassList = new List<SearchDataClass>();

        public Screen[] screens = Screen.AllScreens;

        //public BackgroundWorker backgroundWorker = new BackgroundWorker();

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


        public Form1()
        {
            InitializeComponent();
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
            trackBar1.Value = 50;
            nVolume = trackBar1.Value;
            label16.Text = trackBar1.Value.ToString();
            vlcControl1.Audio.Volume = trackBar1.Value;
            for (int i = 0; i < screens.Length; i++)
            {
                listBox3.Items.Add(screens[i].DeviceName);
            }
            nCurrentItem = 1;
            nLastItem = 0;
            SetFeatureToAllControls(this.Controls);

            CloseConsole();
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

            vlcControl1.Audio.Volume = nVolume;

            if (FullScreenGUI != null)
            {
                foreach (Control control in FullScreenGUI.Controls)
                {
                    if(control.Name == "label1")
                    {
                        control.Text = nVolume.ToString();
                    }
                    if (control.Name == "label5")
                    {
                        control.Text = timestamp;
                    }
                    if (control.Name == "trackBar2")
                    {
                        TrackBar ctrl = (TrackBar)control;
                        ((TrackBar)control).Value = trackBar2.Value;
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
                //Console.Write(ex.ToString());
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

        public void Search(string searchdata, bool bShift, int nIdx)
        {
            try
            {
                Console.Clear();
                Uri address;
                if (bShift)
                    address = new Uri("https://prehraj.to/hledej/" + searchdata + "?vp-page=" + nIdx);
                else
                    address = new Uri("https://prehraj.to/hledej/" + searchdata);

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
                Search(textBox1.Text, false, 0);
            }
            else
            {
                MessageBox.Show("Nic nehledáš negre");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchMovie();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value >= trackBar1.Minimum && trackBar1.Value <= trackBar1.Maximum)
            {
                nVolume = trackBar1.Value;
            }
        }

        private void PlayVideo()
        {
            vlcControl1.Video.IsMouseInputEnabled = false;
            vlcControl1.Video.IsKeyInputEnabled = false;
            if (listBox1.Items.Count > 0)
            {
                try
                {
                    if (nSettings == 0)
                    {
                        string url = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].file;
                        string track = jsondeserializedtracks[listBox2.SelectedIndex >= 0 ? listBox2.SelectedIndex : 0].file;
                        string urllabel = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].label;
                        string tracklabel = jsondeserializedtracks[listBox2.SelectedIndex >= 0 ? listBox2.SelectedIndex : 0].label;
                        var client = new WebClient();
                        client.DownloadFile(track, "subtitle.vtt");
                        string[] opts = { @"sub-file=subtitle.vtt" };
                        vlcControl1.SetMedia(url, opts);
                        vlcControl1.Play();
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
                    else if (nSettings == 1)
                    {
                        string url = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].file;
                        string urllabel = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].label;
                        vlcControl1.SetMedia(url);
                        vlcControl1.Play();
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
                    else if (nSettings == 2)
                    {
                        string url = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].file;
                        string urllabel = jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].label;
                        vlcControl1.SetMedia(url);
                        vlcControl1.Play();
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
                nSettings = 0;
            }
            else if (listBox1.SelectedIndex >= 0)
            {
                nSettings = 1;
            }
            else
            {
                nSettings = 2;
            }
            PlayVideo();
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
            bFullScreen = true;
            if (listBox3.SelectedIndex >= 0)
            {
                Rectangle resolution = screens[listBox3.SelectedIndex].Bounds;
                this.MaximumSize = new Size(resolution.Width, resolution.Height);
                this.Width = resolution.Width;
                this.Height = resolution.Height;

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
                Rectangle resolution = screens[0].Bounds;
                this.MaximumSize = new Size(resolution.Width, resolution.Height);
                this.Width = resolution.Width;
                this.Height = resolution.Height;

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

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.ClearSelected();
            listBox2.ClearSelected();
            listBox3.ClearSelected();
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
                            control.Text = "Konec pauzy";
                        else
                            control.Text = "Pauza";
                    }
                }
            }
            if (bPause)
            {
                button4.Text = "Konec pauzy";
                nStopPosition = trackBar2.Value;
                int mins = nStopPosition / 60;
                int secs = nStopPosition % 60;
            }
            else
            {
                int mins = nStopPosition / 60;
                int secs = nStopPosition % 60;
                Console.WriteLine("Pause position restored at: " + mins + ":" + secs);
                button4.Text = "Pauza";
                bPauseCheck = true;
                PlayVideo();
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
                if (listBox3.SelectedIndex >= 0)
                {
                    Rectangle resolution = screens[listBox3.SelectedIndex].Bounds;
                    this.MaximumSize = new Size(resolution.Width, resolution.Height);
                    this.Width = resolution.Width;
                    this.Height = resolution.Height;

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
                    Rectangle resolution = screens[0].Bounds;
                    this.MaximumSize = new Size(resolution.Width, resolution.Height);
                    this.Width = resolution.Width;
                    this.Height = resolution.Height;

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
                if(bFullScreenGUI)
                    ShowFullScreenGUI();

                trackBar1.Value = vlcControl1.Audio.Volume;

                this.MaximumSize = formOriginalSize;
                this.Width = formOriginalSize.Width;
                this.Height = formOriginalSize.Height;

                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                WindowState = FormWindowState.Normal;
                TopMost = false;

                vlcControl1.Location = vlcOriginalPoint;

                vlcControl1.Width = vlcOriginalSize.Width;
                vlcControl1.Height = vlcOriginalSize.Height;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && e.Modifiers == Keys.Control)
            {
                PauseVideo();
            }
            else if (e.KeyCode == Keys.F && e.Modifiers == Keys.Control)
            {
                CallFullScreen();
            }
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    bFullScreen = false;
                    CloseFSGUI();
                    this.MaximumSize = formOriginalSize;
                    this.Width = formOriginalSize.Width;
                    this.Height = formOriginalSize.Height;

                    FormBorderStyle = FormBorderStyle.FixedToolWindow;
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
                    SearchMovie();
                    break;
                default:
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Na celou obrazovku - [CTRL + F]\nPauza - [CTRL + SPACE]\nFullscreen gui - [F1]\nOpustit celou obrazovku - [ESC]");
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

        private void listBox4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            { 
                if (listBox4.Items.Count > 0)
                {
                    if (listBox4.SelectedItem != null)
                    {
                        int nSelected = listBox4.SelectedIndex;
                        bSelectedIndex = true;
                        label18.Text = "[" + nSelected + "] " + SearchDataClassList[nSelected].label;
                        strUrl = "https://prehraj.to" + SearchDataClassList[nSelected].file;
                        if (!ReadURL())
                        {
                            MessageBox.Show("Chyba při načítání URL!");
                        }
                    }
                }
            }
            else
            {
                bSelectedIndex = false;
            }
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

            label8.Text = SearchDataClassList[index].length;
            label11.Text = SearchDataClassList[index].size;
            label12.Text = SearchDataClassList[index].likes.ToString();
            label14.Text = SearchDataClassList[index].quality.ToString();

            pictureBox1.ImageLocation = SearchDataClassList[index].fileimg;

            if(!bSelectedIndex)
                listBox4.SelectedIndex = index;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (nCurrentItem > 1)
            {
                if (textBox1.TextLength > 0)
                {
                    nCurrentItem -= 1;
                    label15.Text = nCurrentItem + "/" + nLastItem;
                    Search(textBox1.Text, true, nCurrentItem);
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
                    Search(textBox1.Text, true, nCurrentItem);
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

        private void button9_Click(object sender, EventArgs e)
        {
            Link = new Form4(this);
            if (listBox1.Items.Count > 0)
            { 
                Link.SetUrl(jsondeserializedsources[listBox1.SelectedIndex >= 0 ? listBox1.SelectedIndex : 0].file);
                if (listBox4.SelectedIndex >= 0)
                    Link.SetMovName(SearchDataClassList[listBox4.SelectedIndex].label);
                else
                    Link.SetMovName("Unknown");
            }
            Link.Show();
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
            if(e.Button != MouseButtons.Right)
                CallFullScreen();
        }

        private void vlcControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if(bFullScreen)
                    ShowFullScreenGUI();
            }
        }
    }
}
