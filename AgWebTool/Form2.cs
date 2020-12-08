using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vlc.DotNet.Core.Interops;
using Vlc.DotNet.Forms;

namespace AgWebTool
{
    public partial class Form2 : Form
    {
        Form1 MainForm;
        public Form2(Form1 form)
        {
            this.MainForm = form;

            InitializeComponent();

            switch (MainForm.strTheme)
            {
                case "None":
                    break;
                case "Red":
                    button1.BackColor = Color.FromArgb(255, 66, 88);
                    button2.BackColor = Color.FromArgb(255, 66, 88);
                    panel1.BackColor = Color.FromArgb(35, 20, 70);
                    panel2.BackColor = Color.FromArgb(255, 66, 88);
                    label1.BackColor = Color.FromArgb(35, 20, 70);
                    label4.BackColor = Color.FromArgb(35, 20, 70);
                    label5.BackColor = Color.FromArgb(35, 20, 70);
                    trackBar1.BackColor = Color.FromArgb(35, 20, 70);
                    trackBar2.BackColor = Color.FromArgb(35, 20, 70);
                    break;
                case "Green":
                    button1.BackColor = Color.Teal;
                    button2.BackColor = Color.Teal;
                    panel1.BackColor = Color.FromArgb(15, 40, 50);
                    panel2.BackColor = Color.Teal;
                    label1.BackColor = Color.FromArgb(15, 40, 50);
                    label4.BackColor = Color.FromArgb(15, 40, 50);
                    label5.BackColor = Color.FromArgb(15, 40, 50);
                    trackBar1.BackColor = Color.FromArgb(15, 40, 50);
                    trackBar2.BackColor = Color.FromArgb(15, 40, 50);
                    break;
                default:
                    break;
            }

            this.Opacity = 0.75;
            label1.Text = MainForm.nVolume.ToString();
            trackBar1.Value = MainForm.nVolume;
            SetFeatureToAllControls(this.Controls);
            trackBar2.Maximum = (int)MainForm.nMaxVideoLength;
            if (MainForm.bPause)
                button1.Text = "⏯️";
            else
                button1.Text = "⏸";

            foreach (Control cc in Controls)
            {
                cc.MouseDown += new MouseEventHandler(OnMouseDown);
                cc.MouseUp += new MouseEventHandler(OnMouseUp);
                cc.MouseMove += new MouseEventHandler(OnMouseMove);
            }
        }

        public Form2()
        {
            InitializeComponent();
        }

        private Point _mouseDown;
        private Point _formLocation;
        private bool _capture;

        protected void OnMouseDown(object o, MouseEventArgs e)
        {
            _capture = true;
            _mouseDown = e.Location;
            _formLocation = ((Form)TopLevelControl).Location;
        }

        protected void OnMouseUp(object o, MouseEventArgs e)
        {
            _capture = false;
        }

        protected void OnMouseMove(object o, MouseEventArgs e)
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

        private void SetControlEventFocus(object sender, EventArgs e)
        {
            this.ActiveControl = label4;
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

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            MainForm.bTrackHold = true;
            int nVal = trackBar2.Value;
            float pos = nVal / MainForm.nMaxVideoLength;
            MainForm.vlcControl1.Position = pos;
            MainForm.bTrackHold = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            MainForm.bVolumeHold = true;
            if (trackBar1.Value >= trackBar1.Minimum && trackBar1.Value <= trackBar1.Maximum)
            {
                MainForm.nVolume = trackBar1.Value;
            }
            MainForm.bVolumeHold = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.PauseVideo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainForm.CallFullScreen();
        }
    }
}
