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
            label1.Text = MainForm.nVolume.ToString();
            trackBar1.Value = MainForm.nVolume;
            SetFeatureToAllControls(this.Controls);
            trackBar2.Maximum = (int)MainForm.nMaxVideoLength;
            if (MainForm.bPause)
                button1.Text = "Konec pauzy";
            else
                button1.Text = "Pauza";
        }

        public Form2()
        {
            InitializeComponent();
        }

        private Point _mouseDown;
        private Point _formLocation;
        private bool _capture;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _capture = true;
            _mouseDown = e.Location;
            _formLocation = ((Form)TopLevelControl).Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _capture = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
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
