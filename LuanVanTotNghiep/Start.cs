using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuanVanTotNghiep
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FingerCompare f = new FingerCompare();
            f.ShowDialog();
            f = null;
            this.Show();
        }
    }
}
