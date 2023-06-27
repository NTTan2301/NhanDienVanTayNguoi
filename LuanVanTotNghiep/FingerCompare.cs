using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Data.Common;
using static System.Net.WebRequestMethods;
using System.Reflection;
using LuanVanTotNghiep;
using AForge.Video;
using AForge.Video.DirectShow;
using LuanVanTotNghiep.Models;
using LuanVanTotNghiep.Training;

namespace LuanVanTotNghiep
{
    
    public partial class FingerCompare : Form
    {
        private FilterInfoCollection cameras;
        private VideoCaptureDevice cam;
        public FingerCompare()
        {
            InitializeComponent();
            UpdateMaskGaborCollection();
        }

        #region Options info

        //padding
        private int left1 = 50;
        private int top1 = 20;
        private int right1 = 50;
        private int bottom1 = 20;
        private int left2 = 50;
        private int top2 = 20;
        private int right2 = 50;
        private int bottom2 = 20;

        //image process
        public static int m = 50;
        public static int v = 300;
        public static int threshold = 0;
        public static int f = 7;
        public static int fi = 3;

        //mask gabor
        public static ArrayList MaskGaborCollection = new ArrayList();
        public static int maskNumber = 32;

        //compare options
        private int angleLimit = 5;
        private int distanceLimit = 5;
        private int minuNumberLimit = 14;

        #endregion

        private bool isFirst1 = true;
        private int width1, height1;
        private int width2, height2;
        private bool isFirst2 = true;

        private ArrayList minus1 = new ArrayList();
        private ArrayList minus2 = new ArrayList();
        private Minutiae minuResult;
        private string sResult;
        public static int widthSquare = 4;//
        private ImageData image1;
        private int[,] image1Data;
        private ImageData image2;
        private int[,] image2Data;
        private double[,] directMatrix1;
        private System.Windows.Forms.StatusBarPanel statueP2;
        //private DataGridView dgv1;

        private double[,] directMatrix2;

        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-MM0H4N8\SQLEXPRESS;Initial Catalog=LuanVanTotNghiep;Integrated Security=True");

        private void UpdateMaskGaborCollection()
        {
            double direct = 0;
            MaskGaborCollection.Clear();
            for (int i = 0; i < maskNumber; i++)
            {
                MaskGabor mask = new MaskGabor(widthSquare, direct, 1.0 / f, fi);
                MaskGaborCollection.Add(mask);
                direct += Math.PI / maskNumber;
            }
        }
        public void LoadData()
        {
            
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("select * from Information", conn);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dgv1.DataSource = dt;
            conn.Close();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    image1 = new ImageData(dlg.FileName, widthSquare);
                    pic1.Image = image1.ToBitmap();
                    directMatrix1 = image1.Direct;
                    width1 = image1.Width;
                    height1 = image1.Height;
                    minus1.Clear();
                    sResult = "";
                    isFirst1 = true;
                    image1Data = new int[image1.Width, image1.Height];
                    for (int i = 0; i < image1.Width; i++)
                        for (int j = 0; j < image1.Height; j++)
                            image1Data[i, j] = image1.Image[i, j];
                    path = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                }
            }
            catch
            {
                MessageBox.Show("Lỗi khi mở file");
            }
        }

        private void FingerCompare_Load(object sender, EventArgs e)
        {
            LoadData();
            btnLayMau.Hide();
            btnSoSanh.Hide();
        }


        public Boolean SoSanh(string path)
        {
            ImageData image3;

            int[,] image3Data;

            image3 = new ImageData(path, widthSquare);
            // pictureBox2.Image = image3.ToBitmap();
            directMatrix2 = image3.Direct;
            width2 = image3.Width;
            height2 = image3.Height;
            isFirst2 = true;
            minus2.Clear();
            sResult = "";
            image2Data = new int[width2, height2];
            for (int i = 0; i < image3.Width; i++)
                for (int j = 0; j < image3.Height; j++)
                    image2Data[i, j] = image3.Image[i, j];

            try
            {
                int i, j;
                if (!isFirst1)
                {
                    int[,] input1 = new int[width1, height1];
                    for (i = 0; i < width1; i++)
                        for (j = 0; j < height1; j++)
                            input1[i, j] = image1Data[i, j];

                    image1.SetImage(input1, width1, height1, widthSquare);
                }
                if (!isFirst2)
                {
                    int[,] input2 = new int[width2, height2];
                    for (i = 0; i < width2; i++)
                        for (j = 0; j < height2; j++)
                            input2[i, j] = image2Data[i, j];
                    image3.SetImage(input2, width2, height2, widthSquare);
                }
                this.Cursor = Cursors.WaitCursor;

                #region prepare data
                //statueP2.Text = "Đang chuẩn hóa";
                image1.ToNornal(m, v);
                image3.ToNornal(m, v);

                //statueP2.Text = "Đang tăng cường";
                image1.ToFiltring(widthSquare, f, fi);
                image3.ToFiltring(widthSquare, f, fi);

                // statueP2.Text = "Đang nhị phân hóa";
                image1.ToBinary(threshold);
                image3.ToBinary(threshold);

                //statueP2.Text = "Đang làm mảnh";
                image1.ToBoneImage();
                image3.ToBoneImage();

                minus1.Clear();
                minus2.Clear();
                minus1 = image1.GetMinutiae(left1, top1, right1, bottom1);
                minus2 = image3.GetMinutiae(left2, top2, right2, bottom2);
                #endregion

                #region run hough
                //int i;			
                //angles
                int angleStart = -30;
                int angleUnit = 3;
                int angleFinish = 30;
                int anglesCount = Convert.ToInt32((angleFinish - angleStart) / angleUnit) + 1;
                int[] angleSet = new int[anglesCount];
                i = 0;
                int angle = angleStart;
                while (i < anglesCount)
                {
                    angleSet[i] = angle;
                    i++;
                    angle += angleUnit;
                }
                //DELTAXSET
                int deltaXStart = -image1.Width;
                int deltaXFinish = image1.Width;
                int deltaXUnit = 2;
                int deltaXCount = Convert.ToInt32((deltaXFinish - deltaXStart) / deltaXUnit) + 1;
                int[] deltaXSet = new int[deltaXCount];
                i = 0;
                int deltaX = deltaXStart;
                while (i < deltaXCount)
                {
                    deltaXSet[i] = deltaX;
                    i++;
                    deltaX += deltaXUnit;
                }
                //DELTAYSET
                int deltaYStart = -image1.Height;
                int deltaYFinish = image1.Height;
                int deltaYUnit = 2;
                int deltaYCount = Convert.ToInt32((deltaYFinish - deltaYStart) / deltaYUnit) + 1;
                int[] deltaYSet = new int[deltaYCount];
                i = 0;
                int deltaY = deltaYStart;
                while (i < deltaYCount)
                {
                    deltaYSet[i] = deltaY;
                    i++;
                    deltaY += deltaYUnit;
                }

                //scaleset
                double[] scaleSet = { 0.8, 0.9, 1.0, 1.1, 1.2 };
                minuResult = Functions.GetMinutiaeChanging_UseHoughTransform(minus1, minus2, angleSet, deltaXSet, deltaYSet, angleLimit * Math.PI / 180, image1.Width / 2, image1.Height / 2);
                int count = Functions.CountMinuMatching(minus1, minus2, minuResult, distanceLimit, angleLimit * Math.PI / 180);

                this.Cursor = Cursors.Default;
                isFirst1 = false;
                isFirst2 = false;

                if (count >= minuNumberLimit)
                    //MessageBox.Show("Hai vân tay trùng khớp" + minuNumberLimit.ToString());
                    return true;
                else
                    //MessageBox.Show("Hai vân tay không trùng khớp" + minuNumberLimit.ToString());
                    return false;
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý: " + ex.ToString());
                return false;
            }

        }

        public Boolean SoSanh1(Image image)
        {
            ImageData image3;

            int[,] image3Data;

            image3 = new ImageData(image, widthSquare);
            // pictureBox2.Image = image3.ToBitmap();
            directMatrix2 = image3.Direct;
            width2 = image3.Width;
            height2 = image3.Height;
            isFirst2 = true;
            minus2.Clear();
            sResult = "";
            image2Data = new int[width2, height2];
            for (int i = 0; i < image3.Width; i++)
                for (int j = 0; j < image3.Height; j++)
                    image2Data[i, j] = image3.Image[i, j];

            try
            {
                int i, j;
                if (!isFirst1)
                {
                    int[,] input1 = new int[width1, height1];
                    for (i = 0; i < width1; i++)
                        for (j = 0; j < height1; j++)
                            input1[i, j] = image1Data[i, j];

                    image1.SetImage(input1, width1, height1, widthSquare);
                }
                if (!isFirst2)
                {
                    int[,] input2 = new int[width2, height2];
                    for (i = 0; i < width2; i++)
                        for (j = 0; j < height2; j++)
                            input2[i, j] = image2Data[i, j];
                    image3.SetImage(input2, width2, height2, widthSquare);
                }
                this.Cursor = Cursors.WaitCursor;

                #region prepare data
                //statueP2.Text = "Đang chuẩn hóa";
                image1.ToNornal(m, v);
                image3.ToNornal(m, v);

                //statueP2.Text = "Đang tăng cường";
                image1.ToFiltring(widthSquare, f, fi);
                image3.ToFiltring(widthSquare, f, fi);

                // statueP2.Text = "Đang nhị phân hóa";
                image1.ToBinary(threshold);
                image3.ToBinary(threshold);

                //statueP2.Text = "Đang làm mảnh";
                image1.ToBoneImage();
                image3.ToBoneImage();

                minus1.Clear();
                minus2.Clear();
                minus1 = image1.GetMinutiae(left1, top1, right1, bottom1);
                minus2 = image3.GetMinutiae(left2, top2, right2, bottom2);
                #endregion

                #region run hough
                //int i;			
                //angles
                int angleStart = -30;
                int angleUnit = 3;
                int angleFinish = 30;
                int anglesCount = Convert.ToInt32((angleFinish - angleStart) / angleUnit) + 1;
                int[] angleSet = new int[anglesCount];
                i = 0;
                int angle = angleStart;
                while (i < anglesCount)
                {
                    angleSet[i] = angle;
                    i++;
                    angle += angleUnit;
                }
                //DELTAXSET
                int deltaXStart = -image1.Width;
                int deltaXFinish = image1.Width;
                int deltaXUnit = 2;
                int deltaXCount = Convert.ToInt32((deltaXFinish - deltaXStart) / deltaXUnit) + 1;
                int[] deltaXSet = new int[deltaXCount];
                i = 0;
                int deltaX = deltaXStart;
                while (i < deltaXCount)
                {
                    deltaXSet[i] = deltaX;
                    i++;
                    deltaX += deltaXUnit;
                }
                //DELTAYSET
                int deltaYStart = -image1.Height;
                int deltaYFinish = image1.Height;
                int deltaYUnit = 2;
                int deltaYCount = Convert.ToInt32((deltaYFinish - deltaYStart) / deltaYUnit) + 1;
                int[] deltaYSet = new int[deltaYCount];
                i = 0;
                int deltaY = deltaYStart;
                while (i < deltaYCount)
                {
                    deltaYSet[i] = deltaY;
                    i++;
                    deltaY += deltaYUnit;
                }

                //scaleset
                double[] scaleSet = { 0.8, 0.9, 1.0, 1.1, 1.2 };
                minuResult = Functions.GetMinutiaeChanging_UseHoughTransform(minus1, minus2, angleSet, deltaXSet, deltaYSet, angleLimit * Math.PI / 180, image1.Width / 2, image1.Height / 2);
                int count = Functions.CountMinuMatching(minus1, minus2, minuResult, distanceLimit, angleLimit * Math.PI / 180);

                this.Cursor = Cursors.Default;
                isFirst1 = false;
                isFirst2 = false;

                if (count >= minuNumberLimit)
                    //MessageBox.Show("Hai vân tay trùng khớp" + minuNumberLimit.ToString());
                    return true;
                else
                    //MessageBox.Show("Hai vân tay không trùng khớp" + minuNumberLimit.ToString());
                    return false;
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý: " + ex.ToString());
                return false;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            try
            {
                string ngaysinh = dtpNgaySinh.Value.ToString("yyyy/MM/dd");
                string cmnd = txtCMND.Text;
                string hoten = txtTen.Text;

                string gioitinh = txtGioiTinh.Text;
                string quoctich = txtQuocTich.Text;
                byte[] image = ImageToByte(pic2.Image);
                

                string sql = "insert into Information(Cmnd,[Name],Date,Sex,Nationality,[Image]) values (@Cmnd,@Name,@Date,@Sex,@Nationality,@Image)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Cmnd", cmnd);
                cmd.Parameters.AddWithValue("@Name", hoten);
                cmd.Parameters.AddWithValue("@Date", ngaysinh);
                cmd.Parameters.AddWithValue("@Sex", gioitinh);
                cmd.Parameters.AddWithValue("@Nationality", quoctich);
                cmd.Parameters.AddWithValue("@Image", image);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Them moi thanh cong");

            }
            catch (Exception)
            {
                MessageBox.Show(" that bai");
                throw;
            }
            conn.Close();
            LoadData();

        }
        string path = "";
        private void pic2_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                pic2.Image = Image.FromFile(open.FileName);
                this.Text = open.FileName;

            }
        }

        private void dgv1_Click(object sender, EventArgs e)
        {
            int chon = dgv1.CurrentCell.RowIndex;
            txtCMND.Text = dgv1.Rows[chon].Cells[0].Value.ToString();
            txtTen.Text = dgv1.Rows[chon].Cells[1].Value.ToString();
            dtpNgaySinh.Text = dgv1.Rows[chon].Cells[2].Value.ToString();
            txtGioiTinh.Text = dgv1.Rows[chon].Cells[3].Value.ToString();
            txtQuocTich.Text = dgv1.Rows[chon].Cells[4].Value.ToString();

            byte[] image = (byte[])dgv1.Rows[chon].Cells[5].Value;
            pic2.Image = ByteToImage(image);
        }

        public double compare(ArrayList minus1, ArrayList minus2)
        {
            //ham doi sanh mau van tay dau vao(minus1) voi mau van tay co trong datatable(minus2)
            int angleStart = -30;
            int angleUnit = 3;
            int angleFinish = 30;
            int anglesCount = Convert.ToInt32((angleFinish - angleStart) / angleUnit) + 1;
            int[] angleSet = new int[anglesCount];
            int i = 0;
            int angle = angleStart;
            while (i < anglesCount)
            {
                angleSet[i] = angle;
                i++;
                angle += angleUnit;
            }
            //DELTAXSET
            int deltaXStart = -image1.Width;
            int deltaXFinish = image1.Width;
            int deltaXUnit = 2;
            int deltaXCount = Convert.ToInt32((deltaXFinish - deltaXStart) / deltaXUnit) + 1;
            int[] deltaXSet = new int[deltaXCount];
            i = 0;
            int deltaX = deltaXStart;
            while (i < deltaXCount)
            {
                deltaXSet[i] = deltaX;
                i++;
                deltaX += deltaXUnit;
            }
            //DELTAYSET
            int deltaYStart = -image1.Height;
            int deltaYFinish = image1.Height;
            int deltaYUnit = 2;
            int deltaYCount = Convert.ToInt32((deltaYFinish - deltaYStart) / deltaYUnit) + 1;
            int[] deltaYSet = new int[deltaYCount];
            i = 0;
            int deltaY = deltaYStart;
            while (i < deltaYCount)
            {
                deltaYSet[i] = deltaY;
                i++;
                deltaY += deltaYUnit;
            }

            //scaleset
            double[] scaleSet = { 0.8, 0.9, 1.0, 1.1, 1.2 };
            minuResult = Functions.GetMinutiaeChanging_UseHoughTransform(minus1, minus2, angleSet, deltaXSet, deltaYSet, angleLimit * Math.PI / 180, image1.Width / 2, image1.Height / 2);
            int count = Functions.CountMinuMatching(minus1, minus2, minuResult, distanceLimit, angleLimit * Math.PI / 180);

            this.Cursor = Cursors.Default;
            isFirst1 = false;
            isFirst2 = false;
            double a = double.Parse(((count * 50)/ minuNumberLimit).ToString());
            return Math.Round(a, 2);
           /* if (count >= minuNumberLimit)
                //MessageBox.Show("Hai vân tay trùng khớp" + minuNumberLimit.ToString());
                return true;
            else
                //MessageBox.Show("Hai vân tay không trùng khớp" + minuNumberLimit.ToString());
                return false;*/
        }

        List<string> listCodeImage = new List<string>();
        LuanVanTotNghiep.Training.Training tra = new LuanVanTotNghiep.Training.Training();
        Model mo = new Model();
        ArrayList array = new ArrayList();
        List<Information> listInformation = new List<Information>();
        private void btnSoSanh_Click(object sender, EventArgs e)
        {
            string maimage = "";
            listInformation = tra.GetListInforMation();
            listCodeImage = tra.GetListCodeImage();
            array = mo.Listmisus1();
            double a = new double();
            List<double> listphantram = new List<double>();
            foreach (string item in listCodeImage)
            {
                array = mo.Listmisus(item);
                a = compare(minus1, array);
                if (a >= 50)
                {
                    maimage = item;
                    break;
                }
                listphantram.Add(a);
            }
            if (maimage!="")
            {
                if(a>95)
                {
                    MessageBox.Show("Tồn tại vân tay đã được huấn luyện trước đó ");
                }   
                else
                {
                    MessageBox.Show("Tồn tại vân tay trong csdl với tỉ lệ giống: " + a + "% ");
                }    
                
                string cmnd = mo.Getcmnd(maimage);
               
                Information nguoi = new Information();
                foreach (Information item in listInformation)
                {
                    if (cmnd.Equals(item.Cmnd))
                    {
                        nguoi = item;
                        
                    }
                }
                byte[] image = nguoi.ImageInfor;
                PersonalDetails f = new PersonalDetails(nguoi.Cmnd, nguoi.Name, nguoi.Date.ToString("dd/MM/yyyy"), nguoi.Sex, nguoi.Nationality, image);
                f.Show();
            }
            else
            {
                double max = 0;
                foreach (double item in listphantram)
                {
                    if (max < item)
                    {
                        max = item;
                    }
                }
                MessageBox.Show("Vân tay này Không tồn tại trong cơ sở dữ liệu. tỉ lệ cao nhất tìm thấy là: "+ max+"%");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtCMND.Text = "";
            txtGioiTinh.Text = "";
            txtQuocTich.Text = "";
            txtTen.Text = "";
            pic1.Image = null;
            pic2.Image = null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //phân tích nhận diện các điểm đặc trưng.
            try
            {
                int i, j;
                if (!isFirst1)
                {
                    int[,] input1 = new int[width1, height1];
                    for (i = 0; i < width1; i++)
                        for (j = 0; j < height1; j++)
                            input1[i, j] = image1Data[i, j];

                    image1.SetImage(input1, width1, height1, widthSquare);
                }
                image1.ToNornal(m, v);
                image1.ToFiltring(widthSquare, f, fi);
                image1.ToBinary(threshold);
                image1.ToBoneImage();
                minus1.Clear();
                minus1 = image1.GetMinutiae(left1, top1, right1, bottom1);
                if (minus1.Count > 0)
                {
                    MessageBox.Show("Hoàn Thành quá trình huấn luyện");
                    btnSoSanh.Show();
                }
                else
                {
                    MessageBox.Show("bại");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    image1 = new ImageData(dlg.FileName, widthSquare);
                    pic1.Image = image1.ToBitmap();
                    directMatrix1 = image1.Direct;
                    width1 = image1.Width;
                    height1 = image1.Height;
                    minus1.Clear();
                    sResult = "";
                    isFirst1 = true;
                    image1Data = new int[image1.Width, image1.Height];
                    for (int i = 0; i < image1.Width; i++)
                        for (int j = 0; j < image1.Height; j++)
                            image1Data[i, j] = image1.Image[i, j];
                    path = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                }
            }
            catch
            {
                MessageBox.Show("Lỗi khi mở file");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void Cam_newFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pic1.Image= bitmap;
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            if (cam != null && cam.IsRunning)
            {
                cam.Stop();
            }
            image1 = new ImageData(pic1.Image, widthSquare);
            pic1.Image = image1.ToBitmap();
            directMatrix1 = image1.Direct;
            width1 = image1.Width;
            height1 = image1.Height;
            minus1.Clear();
            sResult = "";
            isFirst1 = true;
            image1Data = new int[image1.Width, image1.Height];
            for (int i = 0; i < image1.Width; i++)
                for (int j = 0; j < image1.Height; j++)
                    image1Data[i, j] = image1.Image[i, j];

        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (cam != null && cam.IsRunning)
            {
                cam.Stop();
            }
        }

        private void btnLayMau_Click(object sender, EventArgs e)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            try
            {
                int i, j;
                if (!isFirst1)
                {
                    int[,] input1 = new int[width1, height1];
                    for (i = 0; i < width1; i++)
                        for (j = 0; j < height1; j++)
                            input1[i, j] = image1Data[i, j];

                    image1.SetImage(input1, width1, height1, widthSquare);
                }
                image1.ToNornal(m, v);
                image1.ToFiltring(widthSquare, f, fi);
                image1.ToBinary(threshold);
                image1.ToBoneImage();
                minus1.Clear();
                minus1 = image1.GetMinutiae(left1, top1, right1, bottom1);
                byte[] image = ImageToByte(pic1.Image);
                string sql = "insert into Image(CodeFinger, ImageFinger) values(@CodeFinger,@ImageFinger)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CodeFinger", path);
                cmd.Parameters.AddWithValue("@ImageFinger", image);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    string sql1 = "insert into InforImage(Cmnd, CodeFinger) values(@Cmnd, @CodeFinger)";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn);
                    cmd1.Parameters.AddWithValue("@Cmnd", txtCMND.Text);
                    cmd1.Parameters.AddWithValue("@CodeFinger", path);
                    cmd1.ExecuteNonQuery();

                    foreach (Minutiae item in minus1)
                    {
                        string sql2 = "insert into DataFin(CodeFinger,X,Y,Direct) values(@CodeFinger,@X,@Y,@Direct)";
                        SqlCommand cmd2 = new SqlCommand(sql2, conn);
                        cmd2.Parameters.AddWithValue("@CodeFinger", path);
                        cmd2.Parameters.AddWithValue("@X", item.X.ToString());
                        cmd2.Parameters.AddWithValue("@Y", item.Y.ToString());
                        cmd2.Parameters.AddWithValue("@Direct", item.Direct.ToString());
                        cmd2.ExecuteNonQuery();

                    }
                    MessageBox.Show("Lấy Mẫu Thành Công");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void txtCMND_TextChanged(object sender, EventArgs e)
        {
            if (txtCMND.Text=="")
            {
                btnLayMau.Hide();
            }
            else
            {
                btnLayMau.Show();
            }    
        }

        // chuyen image => byte
        byte[] ImageToByte(Image img)
        {
            MemoryStream m = new MemoryStream();
            img.Save(m, System.Drawing.Imaging.ImageFormat.Png);
            return m.ToArray();
        }

        // chuyen tu byte => image
        Image ByteToImage(byte[] b)
        {
            MemoryStream m = new MemoryStream(b);
            return Image.FromStream(m);
        }
    }
}
