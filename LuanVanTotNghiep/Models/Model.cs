using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuanVanTotNghiep.Training;

namespace LuanVanTotNghiep.Models
{
    internal class Model
    {
        //lop thuc thi huan luyen
        LuanVanTotNghiep.Training.Training tra = new LuanVanTotNghiep.Training.Training();
        List<Information> listInformation = new List<Information>();
        List<InforImage> listInforImage = new List<InforImage>();
        List<DataFin> listDataFin = new List<DataFin>();
        List<string> listCodeFinger = new List<string>();
        public Information GetInformation(string cmnd)
        {
            //ham xet thong tin nguoi
            //cmnd: ma so chung minh nhan dan
            Information i = new Information();
            listInformation = tra.GetListInforMation();
            foreach (Information item in listInformation)
            {
                if (item.Cmnd.Equals(cmnd))
                {
                    i = item;
                }
            }
            return i;
        }

        public string Getcmnd(String codeFinger)
        {
            //ham tim kiem thong tin cmnd
            //codeFinger: ma van tay 
            string i = "";
            listInforImage = tra.GetListInforImage();
            foreach (InforImage item in listInforImage)
            {
                if (codeFinger.Equals(item.CodeFinger))
                {
                    i = item.Cmnd;
                }
            }
            return i;
        }

        public ArrayList Listmisus1()
        {
            //ham huan luyen xac dinh nguoi va mau van tay
            ArrayList arrayList = new ArrayList();
            listDataFin = tra.GetListDataFin();
            listCodeFinger = tra.GetListCodeImage();
            foreach (string CodeFinger in listCodeFinger)
            {
                foreach (DataFin item in listDataFin)
                {
                    if (CodeFinger.Equals(item.CodeFinger))
                    {
                        Minutiae m = new Minutiae();
                        m.X = int.Parse(item.X.ToString());
                        m.Y = int.Parse(item.Y.ToString());
                        m.Direct = double.Parse(item.Direct.ToString());
                        arrayList.Add(m);
                    }
                }
            }
            return arrayList;
        }

        public ArrayList Listmisus(string maimage)
        {
            //ham xac dinh diem dac trung cua tung mau van tay
            //maimgae: ma van tay
            ArrayList arrayList = new ArrayList();
            foreach (DataFin item in listDataFin )
            {
                if (maimage.Equals(item.CodeFinger))
                {
                    Minutiae m = new Minutiae();
                    m.X = int.Parse(item.X.ToString());
                    m.Y = int.Parse(item.Y.ToString());
                    m.Direct = double.Parse(item.Direct.ToString());
                    arrayList.Add(m);
                }
            }
            return arrayList;
        }
    }
}
