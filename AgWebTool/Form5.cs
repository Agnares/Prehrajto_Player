using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgWebTool
{
    public partial class Form5 : Form
    {
        public string Theme;

        public Form5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Theme = "Red";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Theme = "Green";
            this.Close();
        }

        private Point _mouseDown;
        private Point _formLocation;
        private bool _capture;

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            _capture = true;
            _mouseDown = e.Location;
            _formLocation = ((Form)TopLevelControl).Location;
        }

        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            _capture = false;
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
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
