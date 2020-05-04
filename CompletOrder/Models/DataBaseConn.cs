using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CompletOrder.Models
{
     
    public class DataBaseConn
    {
         
        protected MySqlConnection mysqlconn;
        protected string sqlconn;

        protected MySqlConnectionStringBuilder conn_string;
        public DataBaseConn()
        {
            conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = "www.szachownica.com.pl";
            conn_string.Port = 3306;
            conn_string.SslMode = MySqlSslMode.None;
            conn_string.UserID = "root";
            conn_string.Password = "Htyud682f45";
            conn_string.Database = "szachownica";

            Connect();
        }

        private void Connect()
        {
            var connstring = $"SERVER={funkcje.serwer};DATABASE={funkcje.database};TRUSTED_CONNECTION=No; UID={funkcje.sqluser}; PWD={funkcje.password};Connection Timeout=30";


            MySqlConnection connection = new MySqlConnection(conn_string.ToString());
            this.mysqlconn = connection;
            sqlconn = connstring;
        }


        class funkcje
        {
            public static int wersja_nr = 20193;
            //public static int SessionID;
            public static string serwer = "10.8.0.6";
            public static string database = "cdnxl_joart";//cdnxl_joart ; cdnxl_test
            public static string sqluser = "sa";
            public static string password = "sqlSQL123#";
            //public static int dokid;
            public static int typZamknij = 0; // //1-bufor , 0- zatwierdź ////////////////////// 
        }

    }

     
     
    
   
}
