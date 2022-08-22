
using MySqlConnector;
using Xamarin.Forms;

namespace CompletOrder.Models
{

    public class DataBaseConn  
    {
        

        protected MySqlConnection mysqlconn;
        protected string sqlconn;

        protected MySqlConnectionStringBuilder conn_string;
        public DataBaseConn()
        {
                  

            Connect();
        }

        internal void Connect()
        {
              sqlconn = $"SERVER={funkcje.serwer};" +
                $"DATABASE={funkcje.database};" +
                $"TRUSTED_CONNECTION=No; UID={funkcje.sqluser}; " +
                $"PWD={funkcje.password};Connection Timeout=30";

            conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = "146.59.85.82";
            conn_string.Port = 3306;
            conn_string.SslMode = MySqlSslMode.None;
            conn_string.UserID = ((App)Application.Current).BaseName;
            conn_string.Password = ((App)Application.Current).PasswordSQL;
            //conn_string.Database = "32610188_df84ef7f";
            conn_string.Database = "admin_hhd663ehd";

            MySqlConnection connection = new MySqlConnection(conn_string.ToString());
            this.mysqlconn = connection;
             
        }


        class funkcje
        {
            public static int wersja_nr = 20193;
            //public static int SessionID;
            public static string serwer = "192.168.1.55";
            public static string database = "cdnxl_tst";//cdnxl_joart ; cdnxl_test
            public static string sqluser = "sa";
            public static string password = "sql";
            //public static int dokid;
            public static int typZamknij = 0; // //1-bufor , 0- zatwierdź ////////////////////// 
        }

    }

     
     
    
   
}
