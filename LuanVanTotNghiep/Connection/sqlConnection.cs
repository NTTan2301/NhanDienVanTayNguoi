using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace LuanVanTotNghiep.Connection
{
    internal class sqlConnection
    {
        public SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-MM0H4N8\SQLEXPRESS;Initial Catalog=LuanVanTotNghiep;Integrated Security=True");
        public SqlConnection GetConnection()
        { 
            return conn; 
        }

    }
}
