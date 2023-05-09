using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintApp
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A simple Paint-like app that allows users to create basic graphics and images.\nThe app provides a canvas where users can draw and paint various shapes and colors using built-in tools such as pencils, erasers, and shapes.\nThe app also allows users to save their artwork in different formats.\n\nCreated by Stefan Gogov\nFacNum: 2201681037", "Info");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
           // start the main form
           Paint mainForm = new Paint();
           this.Hide();
            mainForm.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
