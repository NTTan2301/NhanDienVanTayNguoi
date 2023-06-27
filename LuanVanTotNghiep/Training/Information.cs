using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuanVanTotNghiep.Training
{
    internal class Information
    {
        //lop doi tuong information
        private string cmnd;
        private string name;
        private DateTime date;
        private string sex;
        private string nationality;
        private byte[] imageInfor;

        public string Cmnd { get => cmnd; set => cmnd = value; }
        public string Name { get => name; set => name = value; }
        public string Sex { get => sex; set => sex = value; }
        public string Nationality { get => nationality; set => nationality = value; }
        public byte[] ImageInfor { get => imageInfor; set => imageInfor = value; }
        public DateTime Date { get => date; set => date = value; }
    }
}
