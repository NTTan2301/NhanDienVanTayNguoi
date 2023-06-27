using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LuanVanTotNghiep
{
    public partial class PersonalDetails : Form
    {
        public PersonalDetails()
        {
            InitializeComponent();
        }
        string cmnd, hoten, ngaysinh, gioitinh, quoctich;

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        byte[] image;
        public PersonalDetails(string cmnd, string hoten, string ngaysinh, string gioitinh, string quoctich, byte[] image)
        {  
            InitializeComponent();
            this.cmnd = cmnd; 
            this.hoten = hoten; 
            this.ngaysinh= ngaysinh;
            this.gioitinh= gioitinh;
            this.quoctich= quoctich;
            this.image = image;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            lbCmnd.Text = cmnd; 
            lbHoten.Text = hoten;
            lbGioitinh.Text = gioitinh;
            lbNgaysinh.Text = ngaysinh;
            lbQuoctich.Text = quoctich;
            picImage.Image = ByteToImage(image);
            string maQRCODE = cmnd + " || " + hoten + " | " + ngaysinh + " | " + gioitinh + " | " + quoctich;
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(maQRCODE, QRCodeGenerator.ECCLevel.Q);

            QRCode qRCode = new QRCode(qRCodeData);
            picCode.Image = qRCode.GetGraphic(10);
        }

        Image ByteToImage(byte[] b)
        {
            MemoryStream m = new MemoryStream(b);
            return Image.FromStream(m);
        }
    }
}
