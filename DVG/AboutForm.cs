using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVG
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            label1.Text = "This software was made with the help of mindfusion library \n"+
                "It was made for personal use and I wanted to share it with others \n"+
                "Hope it will help you";

            pictureBox1.Image = Properties.Resources.mindfusion_logo;
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.Image = Properties.Resources.GEA_logo;
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            this.Icon = Properties.Resources.dvg_logo;

            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://mindfusion.eu/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://georgealromhin.ga");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
