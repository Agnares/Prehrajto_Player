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

        DateTime startedAt;

        private WebClient wc = new WebClient();

        public Form4(Form1 form)
        {
            this.MainForm = form;
            InitializeComponent();
        }
        public Form4()
        {
            InitializeComponent();
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
                if((long)timeSpan.TotalSeconds > 0)
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
    }
}
