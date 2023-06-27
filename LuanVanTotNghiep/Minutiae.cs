using System;

namespace LuanVanTotNghiep
{
    /// <summary>
    /// Summary description for Minutiae.
    /// </summary>
    public class Minutiae
    {
        //lop diem dac trung
        private int x;
        private int y;
        private double direct;

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public double Direct { get => direct; set => direct = value; }

        #region properties
        /*public int X
        {
            get
            {
                return x;
            }
        }
        public int Y
        {
            get
            {
                return y;
            }
        }
        public double Direct
        {
            get
            {
                return direct;
            }
        }*/
        #endregion

        #region Contructor
        public Minutiae()
        {
        }
        public Minutiae(int _x, int _y, double _direct)
        {
            X = _x;
            Y = _y;
            Direct = _direct;
        }
        #endregion

        public void SetMinutiae(int _x, int _y, double _direct)
        {
            X = _x;
            Y = _y;
            Direct = _direct;
        }

        public Minutiae GetMinutiaeAfterChange(int deltaX, int deltaY, double angleRotation, int xRoot, int yRoot)
        {
            //ham tim vi tri tuong doi cac diem dac trung co trong anh
            int xCurrent = X - xRoot;
            int yCurrent = yRoot - Y;
            int _x = Convert.ToInt32(xCurrent * Math.Cos(angleRotation) - yCurrent * Math.Sin(angleRotation)) + deltaX;
            int _y = Convert.ToInt32(xCurrent * Math.Sin(angleRotation) + yCurrent * Math.Cos(angleRotation)) + deltaY;
            double _direct = Direct + angleRotation;
            if (_direct > Math.PI)
                _direct = _direct - Math.PI;
            return new Minutiae(xRoot + _x, yRoot - _y, _direct);
        }
    }
}
