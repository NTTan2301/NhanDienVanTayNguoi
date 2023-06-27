using LuanVanTotNghiep.Connection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LuanVanTotNghiep.Training
{
    internal class Training
    {
        //lop huan luyen 
        sqlConnection con = new sqlConnection();
        SqlConnection conn = new SqlConnection();
        
        public List<Information> GetListInforMation()
        {
            //lay thong tin danh sach cac doi tuong nguoi.
            List<Information> listInforMation = new List<Information>();
            conn = con.GetConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = "select * from information";
            SqlCommand cmd = new SqlCommand(sql,conn);
            SqlDataAdapter da= new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Information info = new Information();
                info.Cmnd = dt.Rows[i][0].ToString();
                info.Name = dt.Rows[i][1].ToString();
                info.Date = (DateTime) dt.Rows[i][2];
                info.Sex = dt.Rows[i][3].ToString();
                info.Nationality = dt.Rows[i][4].ToString();
                //info.ImageInfor = (new ASCIIEncoding()).GetBytes(dt.Rows[i][5].ToString());
                info.ImageInfor  = (byte[])dt.Rows[i][5];
                listInforMation.Add(info);
            }
            return listInforMation;
        }

        public List<InforImage> GetListInforImage()
        {
            //lay thong tin cac doi tuong inforImage
            List<InforImage> listInforImage = new List<InforImage>();
            conn = con.GetConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = "select * from InforImage";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                InforImage im = new InforImage();
                im.Cmnd = dt.Rows[i][1].ToString();
                im.CodeFinger = dt.Rows[i][2].ToString();
                listInforImage.Add(im);
            }
            return listInforImage;
        }

        public List<DataFin> GetListDataFin(string CodeFinger)
        {
            //lay thong tin du lieu diem dac trung
            //CodeFinger: ma van tay 
            List<DataFin> listDataFin = new List<DataFin>();
            conn = con.GetConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = "select * from DataFin where CodeFinger = '"+CodeFinger+"'";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataFin df = new DataFin();
                df.CodeFinger = dt.Rows[i][1].ToString();
                df.X = dt.Rows[i][2].ToString();
                df.Y = dt.Rows[i][3].ToString();
                df.Direct = dt.Rows[i][4].ToString();
                listDataFin.Add(df);
            }
            return listDataFin;
        }
        public List<DataFin> GetListDataFin()
        {
            //lay toan bo du lieu diem dac trung
            List<DataFin> listDataFin = new List<DataFin>();
            conn = con.GetConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = "select * from DataFin";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataFin df = new DataFin();
                df.CodeFinger = dt.Rows[i][1].ToString();
                df.X = dt.Rows[i][2].ToString();
                df.Y = dt.Rows[i][3].ToString();
                df.Direct = dt.Rows[i][4].ToString();
                listDataFin.Add(df);
            }
            return listDataFin;
        }

        public List<string> GetListCodeImage()
        {
            //lay ma cac diem dac trung 
            List<string> listCodeImage = new List<string>();
            string sql = "select DISTINCT CodeFinger from DataFin";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string a = dt.Rows[i][0].ToString();
                listCodeImage.Add(a);
            }
            return listCodeImage;
        }
    }
}
