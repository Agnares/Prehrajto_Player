using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgWebTool
{
    public partial class Form4 : Form
    {
        Form1 MainForm;
        public string url;
        public string mov;
        private bool bSearchBoxClicked;
        private bool bSearchBoxClickedFav;
        private bool bSearching;
        private bool bSearchingFav;
        private bool bLoaded;

        DateTime startedAt;

        private WebClient wc = new WebClient();

        public Form4(Form1 form, int nTab)
        {
            bLoaded = false;
            this.MainForm = form;
            InitializeComponent();
            InitHistory();
            InitFavourite();
            tabControl1.SelectedIndex = nTab;
            bSearchBoxClicked = false;
            bSearchBoxClickedFav = false;
            bSearching = false;
            bSearchingFav = false;
            textBox2.Text = MainForm.strPath;
            checkBox1.Checked = MainForm.GetLoadOnDataStatus();
            bLoaded = true;
        }
        public Form4()
        {
            InitializeComponent();
        }

        public class HistoryData
        {
            public HistoryData(string strID, string strAddr, string strName, string strTime)
            {
                this.strID = strID;
                this.strAddr = strAddr;
                this.strName = strName;
                this.strTime = strTime;
            }
            public string strID { get; set; }
            public string strAddr { get; set; }
            public string strName { get; set; }
            public string strTime { get; set; }
        }

        List<HistoryData> HistoryDataList = new List<HistoryData>();
        List<HistoryData> HistoryDataListTemp = new List<HistoryData>();

        public void InitHistory()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@".\history.txt");
            HistoryDataList.Clear();
            string line;
            while ((line = file.ReadLine()) != null)
            {
                int nStartID = line.IndexOf("<ID>");
                int nEndID = line.IndexOf("</ID>", nStartID);
                string strID = line.Substring(nStartID + 4, nEndID - nStartID - 4);

                int nStartAddr = line.IndexOf("<ADDRESS>");
                int nEndAddr = line.IndexOf("</ADDRESS>", nStartAddr);
                string strAddr = line.Substring(nStartAddr + 9, nEndAddr - nStartAddr - 9);

                int nStartName = line.IndexOf("<NAME>");
                int nEndName = line.IndexOf("</NAME>", nStartName);
                string strName = line.Substring(nStartName + 6, nEndName - nStartName - 6);

                int nStartTime = line.IndexOf("<TIMESTAMP>");
                int nEndTime = line.IndexOf("</TIMESTAMP>", nStartTime);
                string strTime = line.Substring(nStartTime + 11, nEndTime - nStartTime - 11);

                HistoryDataList.Add(new HistoryData(strID, strAddr, strName, strTime));
            }
            file.Close();

            listBox1.Items.Clear();
            for (int i = 0; i < HistoryDataList.Count; i++)
            {
                string strListData = "[" + HistoryDataList[i].strTime + "]" + HistoryDataList[i].strName;
                listBox1.Items.Add(strListData);
            }
        }

        List<HistoryData> FavouriteDataList = new List<HistoryData>();
        List<HistoryData> FavouriteDataListTemp = new List<HistoryData>();

        public void InitFavourite()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@".\favourite.txt");
            FavouriteDataList.Clear();
            string line;
            while ((line = file.ReadLine()) != null)
            {
                int nStartID = line.IndexOf("<ID>");
                int nEndID = line.IndexOf("</ID>", nStartID);
                string strID = line.Substring(nStartID + 4, nEndID - nStartID - 4);

                int nStartAddr = line.IndexOf("<ADDRESS>");
                int nEndAddr = line.IndexOf("</ADDRESS>", nStartAddr);
                string strAddr = line.Substring(nStartAddr + 9, nEndAddr - nStartAddr - 9);

                int nStartName = line.IndexOf("<NAME>");
                int nEndName = line.IndexOf("</NAME>", nStartName);
                string strName = line.Substring(nStartName + 6, nEndName - nStartName - 6);

                int nStartTime = line.IndexOf("<TIMESTAMP>");
                int nEndTime = line.IndexOf("</TIMESTAMP>", nStartTime);
                string strTime = line.Substring(nStartTime + 11, nEndTime - nStartTime - 11);

                FavouriteDataList.Add(new HistoryData(strID, strAddr, strName, strTime));
            }
            file.Close();

            listBox2.Items.Clear();
            for (int i = 0; i < FavouriteDataList.Count; i++)
            {
                string strListData = "[" + FavouriteDataList[i].strTime + "]" + FavouriteDataList[i].strName;
                listBox2.Items.Add(strListData);
            }
        }

        public void SetUrl(string url)
        {
            this.url = url;
            textBox1.Text = this.url;
        }

        public void SetMovName(string mov)
        {
            this.mov = mov;
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            label2.Text = "Staženo: " + e.ProgressPercentage.ToString() + "%";
            var timeSpan = DateTime.Now - startedAt;
            if (timeSpan.TotalSeconds > 0)
            {
                if ((long)timeSpan.TotalSeconds > 0)
                {
                    var bytesPerSecond = e.BytesReceived / (long)timeSpan.TotalSeconds;
                    label3.Text = "Rychlost: " + (bytesPerSecond / 1024).ToString() + " kb/s";
                }
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label2.Text = "Staženo: 0%";
            label3.Text = "Rychlost: 0 mb/s";

            if (e.Cancelled)
            {
                MessageBox.Show("Stahování zastaveno");
                return;
            }

            if (e.Error != null)
            {
                MessageBox.Show("Chyba při stahování videa!");

                return;
            }

            MessageBox.Show("Video úspěšně staženo!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (url != null)
            {
                int nExpiresIndex = this.url.LastIndexOf("?expires");
                int nDotIndex = this.url.LastIndexOf(".");
                string shortenedurl = this.url.Substring(0, this.url.Length - (this.url.Length - nExpiresIndex));
                shortenedurl = shortenedurl.Substring(nDotIndex);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.FileName = mov + shortenedurl;
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    startedAt = DateTime.Now;
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(url), saveFileDialog1.FileName);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            wc.CancelAsync();
        }

        private void RestartApp()
        {
            Application.Restart();
            Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\theme.txt", false))
            {
                file.Write("None");
            }
            RestartApp();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\theme.txt", false))
                    {
                        if (!MainForm.strTheme.Contains("Custom"))
                        {
                            file.Write("Custom:<PATH>" + filePath + "</PATH><THEME>" + MainForm.strTheme + "</THEME>");
                        }
                        else
                        {
                            int nStartTheme = MainForm.strTheme.IndexOf("<THEME>");
                            int nEndTheme = MainForm.strTheme.LastIndexOf("</THEME>");

                            string strThemeSelect = MainForm.strTheme.Substring(nStartTheme + 7, (MainForm.strTheme.Length - nStartTheme) - (MainForm.strTheme.Length - nEndTheme) - 7);
                            file.Write("Custom:<PATH>" + filePath + "</PATH><THEME>" + strThemeSelect + "</THEME>");
                        }
                    }
                }
            }

            RestartApp();
        }

        private void AddListData()
        {
            listBox1.Items.Clear();
            HistoryDataListTemp.Clear();
            for (int i = 0; i < HistoryDataList.Count; i++)
            {
                string strListData = "[" + HistoryDataList[i].strTime + "]" + HistoryDataList[i].strName;
                if (strListData.ToUpper().Contains(textBox3.Text.ToUpper()))
                {
                    listBox1.Items.Add(strListData);
                    HistoryDataListTemp.Add(new HistoryData(HistoryDataList[i].strID, HistoryDataList[i].strAddr, HistoryDataList[i].strName, HistoryDataList[i].strTime));
                }
            }
        }

        private void AddFavouriteData()
        {
            listBox2.Items.Clear();
            FavouriteDataListTemp.Clear();
            for (int i = 0; i < FavouriteDataList.Count; i++)
            {
                string strListData = "[" + FavouriteDataList[i].strTime + "]" + FavouriteDataList[i].strName;
                if (strListData.ToUpper().Contains(textBox4.Text.ToUpper()))
                {
                    listBox2.Items.Add(strListData);
                    FavouriteDataListTemp.Add(new HistoryData(FavouriteDataList[i].strID, FavouriteDataList[i].strAddr, FavouriteDataList[i].strName, FavouriteDataList[i].strTime));
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length > 0)
                bSearching = true;
            else
                bSearching = false;

            AddListData();
        }

        private void textBox3_MouseClick(object sender, MouseEventArgs e)
        {
            if (!bSearchBoxClicked)
            {
                textBox3.Clear();
                bSearchBoxClicked = true;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bSearching = false;
            bSearchingFav = false;
            if (bSearchBoxClicked && textBox3.TextLength == 0)
            {
                bSearchBoxClicked = false;
                textBox3.Text = "Hledat:";
                listBox1.Items.Clear();
                for (int i = 0; i < HistoryDataList.Count; i++)
                {
                    string strListData = "[" + HistoryDataList[i].strTime + "]" + HistoryDataList[i].strName;
                    listBox1.Items.Add(strListData);
                }
            }
            else if (bSearchBoxClickedFav && textBox4.TextLength == 0)
            {
                bSearchBoxClickedFav = false;
                textBox4.Text = "Hledat:";
                listBox2.Items.Clear();
                for (int i = 0; i < FavouriteDataList.Count; i++)
                {
                    string strListData = "[" + FavouriteDataList[i].strTime + "]" + FavouriteDataList[i].strName;
                    listBox2.Items.Add(strListData);
                }
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                if (checkBox1.Checked)
                {
                    MainForm.VideoPlayingPause();
                    if (listBox1.Items.Count > 0)
                    {
                        if (listBox1.SelectedItem != null)
                        {
                            if (bSearching)
                            {
                                MainForm.historyaddr = HistoryDataListTemp[listBox1.SelectedIndex].strAddr;
                                MainForm.Search("", false, true, 1);
                                MainForm.SetLB4SelectedIDX(Int32.Parse(HistoryDataListTemp[listBox1.SelectedIndex].strID));
                                MainForm.UpdateLB4(true);
                            }
                            else
                            {
                                MainForm.historyaddr = HistoryDataList[listBox1.SelectedIndex].strAddr;
                                MainForm.Search("", false, true, 1);
                                MainForm.SetLB4SelectedIDX(Int32.Parse(HistoryDataList[listBox1.SelectedIndex].strID));
                                MainForm.UpdateLB4(true);
                            }
                            MainForm.UpdateLinkData(this);
                        }
                    }
                }
            }
            else
            {
                listBox1.ClearSelected();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            using (var fs = new FileStream(@".\history.txt", FileMode.Truncate))
            {
            }
        }

        private void RemoveHistoryItem(HistoryData data, bool bFavourite)
        {
            string filestr;
            if (bFavourite)
                filestr = @".\favourite.txt";
            else
                filestr = @".\history.txt";

            System.IO.StreamReader file = new System.IO.StreamReader(filestr);
            string line;
            int counter = 0;
            int remline = 0;
            while ((line = file.ReadLine()) != null)
            {
                int nStartID = line.IndexOf("<ID>");
                int nEndID = line.IndexOf("</ID>", nStartID);
                string strID = line.Substring(nStartID + 4, nEndID - nStartID - 4);

                int nStartAddr = line.IndexOf("<ADDRESS>");
                int nEndAddr = line.IndexOf("</ADDRESS>", nStartAddr);
                string strAddr = line.Substring(nStartAddr + 9, nEndAddr - nStartAddr - 9);

                int nStartName = line.IndexOf("<NAME>");
                int nEndName = line.IndexOf("</NAME>", nStartName);
                string strName = line.Substring(nStartName + 6, nEndName - nStartName - 6);

                int nStartTime = line.IndexOf("<TIMESTAMP>");
                int nEndTime = line.IndexOf("</TIMESTAMP>", nStartTime);
                string strTime = line.Substring(nStartTime + 11, nEndTime - nStartTime - 11);

                if (data.strID == strID && data.strAddr == strAddr && data.strName == strName && data.strTime == strTime)
                {
                    remline = counter;
                }
                counter++;
            }
            file.Close();
            var lines = new List<string>(System.IO.File.ReadAllLines(filestr));
            lines.RemoveAt(remline);
            File.WriteAllLines(filestr, lines.ToArray());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                if (bSearching)
                {
                    int nIdx = listBox1.SelectedIndex;
                    listBox1.Items.Clear();
                    for (int i = 0; i < HistoryDataList.Count; i++)
                    {
                        HistoryData selected = HistoryDataList[i];
                        HistoryData temp = HistoryDataListTemp[nIdx];
                        if (selected.strID == temp.strID && selected.strAddr == temp.strAddr && selected.strName == temp.strName && selected.strTime == temp.strTime)
                        {
                            RemoveHistoryItem(HistoryDataList[i], false);
                            HistoryDataList.RemoveAt(i);
                        }
                    }
                    HistoryDataListTemp.RemoveAt(nIdx);
                    AddListData();
                }
                else
                {
                    HistoryDataListTemp.Clear();
                    RemoveHistoryItem(HistoryDataList[listBox1.SelectedIndex], false);
                    HistoryDataList.RemoveAt(listBox1.SelectedIndex);
                    listBox1.Items.Clear();
                    for (int i = 0; i < HistoryDataList.Count; i++)
                    {
                        string strListData = "[" + HistoryDataList[i].strTime + "]" + HistoryDataList[i].strName;
                        listBox1.Items.Add(strListData);
                    }
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length > 0)
                bSearchingFav = true;
            else
                bSearchingFav = false;

            AddFavouriteData();
        }

        private void textBox4_MouseClick(object sender, MouseEventArgs e)
        {
            if (!bSearchBoxClickedFav)
            {
                textBox4.Clear();
                bSearchBoxClickedFav = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string filestr = @".\favourite.txt";
                if (bSearching)
                {
                    int nIdx = listBox1.SelectedIndex;
                    listBox1.Items.Clear();
                    for (int i = 0; i < HistoryDataList.Count; i++)
                    {
                        HistoryData selected = HistoryDataList[i];
                        HistoryData temp = HistoryDataListTemp[nIdx];
                        if (selected.strID == temp.strID && selected.strAddr == temp.strAddr && selected.strName == temp.strName && selected.strTime == temp.strTime)
                        {
                            string[] lines = File.ReadAllLines(filestr);
                            List<string> newLines = new List<string>();

                            newLines.Add("<ID>" + selected.strID + "</ID><ADDRESS>" + selected.strAddr + "</ADDRESS><NAME>" + selected.strName + "</NAME><TIMESTAMP>" + selected.strTime + "</TIMESTAMP>");
                            for (int j = 0; j < lines.Length; j++)
                                newLines.Add(lines[j]);

                            using (var fs = new FileStream(filestr, FileMode.Truncate)) { }

                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filestr, true))
                            {
                                for (int j = 0; j < newLines.Count; j++)
                                    file.WriteLine(newLines[j]);
                            }
                        }
                    }
                    HistoryDataListTemp.RemoveAt(nIdx);
                    AddListData();
                }
                else
                {
                    HistoryDataListTemp.Clear();
                    string[] lines = File.ReadAllLines(filestr);
                    List<string> newLines = new List<string>();

                    HistoryData selected = HistoryDataList[listBox1.SelectedIndex];
                    newLines.Add("<ID>" + selected.strID + "</ID><ADDRESS>" + selected.strAddr + "</ADDRESS><NAME>" + selected.strName + "</NAME><TIMESTAMP>" + selected.strTime + "</TIMESTAMP>");
                    for (int j = 0; j < lines.Length; j++)
                        newLines.Add(lines[j]);

                    using (var fs = new FileStream(filestr, FileMode.Truncate)) { }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(filestr, true))
                    {
                        for (int j = 0; j < newLines.Count; j++)
                            file.WriteLine(newLines[j]);
                    }
                    listBox1.Items.Clear();
                    for (int i = 0; i < HistoryDataList.Count; i++)
                    {
                        string strListData = "[" + HistoryDataList[i].strTime + "]" + HistoryDataList[i].strName;
                        listBox1.Items.Add(strListData);
                    }
                }
                InitFavourite();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            using (var fs = new FileStream(@".\favourite.txt", FileMode.Truncate))
            {
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                if (bSearchingFav)
                {
                    int nIdx = listBox2.SelectedIndex;
                    listBox2.Items.Clear();
                    for (int i = 0; i < FavouriteDataList.Count; i++)
                    {
                        HistoryData selected = FavouriteDataList[i];
                        HistoryData temp = FavouriteDataListTemp[nIdx];
                        if (selected.strID == temp.strID && selected.strAddr == temp.strAddr && selected.strName == temp.strName && selected.strTime == temp.strTime)
                        {
                            RemoveHistoryItem(FavouriteDataList[i], true);
                            FavouriteDataList.RemoveAt(i);
                        }
                    }
                    FavouriteDataListTemp.RemoveAt(nIdx);
                    AddFavouriteData();
                }
                else
                {
                    FavouriteDataListTemp.Clear();
                    RemoveHistoryItem(FavouriteDataList[listBox2.SelectedIndex], true);
                    FavouriteDataList.RemoveAt(listBox2.SelectedIndex);
                    listBox2.Items.Clear();
                    for (int i = 0; i < FavouriteDataList.Count; i++)
                    {
                        string strListData = "[" + FavouriteDataList[i].strTime + "]" + FavouriteDataList[i].strName;
                        listBox2.Items.Add(strListData);
                    }
                }
            }
        }

        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                if (checkBox1.Checked)
                {
                    MainForm.VideoPlayingPause();
                    if (listBox2.Items.Count > 0)
                    {
                        if (listBox2.SelectedItem != null)
                        {
                            if (bSearchingFav)
                            {
                                MainForm.historyaddr = FavouriteDataListTemp[listBox2.SelectedIndex].strAddr;
                                MainForm.Search("", false, true, 1);
                                MainForm.SetLB4SelectedIDX(Int32.Parse(FavouriteDataListTemp[listBox2.SelectedIndex].strID));
                                MainForm.UpdateLB4(true);
                            }
                            else
                            {
                                MainForm.historyaddr = FavouriteDataList[listBox2.SelectedIndex].strAddr;
                                MainForm.Search("", false, true, 1);
                                MainForm.SetLB4SelectedIDX(Int32.Parse(FavouriteDataList[listBox2.SelectedIndex].strID));
                                MainForm.UpdateLB4(true);
                            }
                            MainForm.UpdateLinkData(this);
                        }
                    }
                }
            }
            else
            {
                listBox2.ClearSelected();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (bLoaded)
            { 
                MainForm.bLoadDataOnClick = checkBox1.Checked;
                MainForm.RewriteSettings();
            }
        }
    }
}
