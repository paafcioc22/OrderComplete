using CompletOrder.Models;
using CompletOrder.Views;
using MySql.Data.MySqlClient;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CompletOrder.ViewModels
{
    public class OrderViewModel :DataBaseConn, INotifyPropertyChanged
    {
        private MySqlConnection connection;
        private SQLiteAsyncConnection _connection;

        //public ObservableCollection<Order> OrderList { get; set; }

        private ObservableCollection<Order> _orderList;
        private ObservableCollection<Order> GetOrders;

        public ObservableCollection<Order> OrderList
        {
            get { return _orderList; }
            set
            {
                //_orderList = value;
                SetValue(ref _orderList, value);
               //OnPropertyChanged(nameof(OrderList));
            }
        }


        //public static string NazwaPlatnosci { get; set; }

        private string _nazwaPlatnosci;

        public string NazwaPlatnosci
        {
            get { return _nazwaPlatnosci; }
            set 
            {
                SetValue(ref _nazwaPlatnosci, value);
                PobierzListe();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public OrderViewModel()
        {
            OrderList = new ObservableCollection<Order>();
            GetOrders = new ObservableCollection<Order>(); 

            connection = new MySqlConnection(conn_string.ToString());

            _connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();
           
            PobierzListe();
            OrderList = GetOrders;
        }

        //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;

            OnPropertyChanged(propertyName);
        }

        private string _ileZam;

        public string IleZam
        {
            get { return _ileZam; }
            set
            {
                SetValue(ref _ileZam, value);
                OnPropertyChanged(nameof(IleZam));
            }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                SetValue(ref _filter, value);
                Search();
            }
        }


        private bool _filtr;
        private string _filter;

        public bool Filtr
        {
            get { return _filtr; }
            set 
            {
                
                _filtr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filtr)));
                if (_filtr)
                {
                    PobierzListe();
                    OrderList = GetOrders;
                }
                else { 
                
                    PobierzListe();
                    OrderList = GetOrders;
                }
            }
        }

        public static ObservableCollection<T> Convert2<T>(IList<T> original)
        {
            return new ObservableCollection<T>(original);
        }


        public ICommand SearchCommand => new Command(Search);
        public void Search()
        {
            if (string.IsNullOrWhiteSpace(_filter))
            {
                OrderList = GetOrders;// ItemData.Items;
            }
            else
            {
                var tmp = GetOrders.Where(c => c.id.ToString().Contains(_filter)).ToList();
                OrderList = Convert2(tmp);
            }


            //if (String.IsNullOrWhiteSpace(searchText))
            //    return orderView.OrderList;
            ////return _listaOrder.Where(c => c.id.ToString().Contains(searchText));
            //return orderView.OrderList.Where(c => c.id.ToString().Contains(searchText));
        }


        string UstawFiltry()
        { 
            var filtry = Application.Current as App;
            string statusy = "status in (";
            string daty = "data between 'dataod' and 'datado' and ";
            string platnosc_info = " AND platnosc_info LIKE '%xxxx%'";

            if (filtry!=null)
            {
                if (filtry.FiltrAnulowane)
                    statusy += "'anulowane',";

                if (filtry.FiltrNowe)
                    statusy += "'nowe',";

                if (filtry.FiltrRealizowane)
                    statusy += "'przyjęte do realizacji',";

                if (filtry.FiltrZaplacone)
                    statusy += "'zapłacone',";

                if (filtry.FiltrZrealizowane)
                    statusy += "'zrealizowane',";

                statusy += ")";
                statusy = statusy.Replace(",)", ")");

                if (filtry.WybranyTypPlatnosci != 0)
                {


                    if (SettingsPage._typyPlatnosci != null)
                    {
                        var nazwap = SettingsPage._typyPlatnosci.Where(x => x.Id == filtry.WybranyTypPlatnosci + 1).ToList()[0].Typ_platnosc;
                         
                        var nazwa = string.IsNullOrEmpty(nazwap) ? "" : nazwap.ToLower();
                        platnosc_info = platnosc_info.Replace("xxxx", nazwa);
                    }
                    
                }
                else
                {
                    platnosc_info = "";
                }

                DateTimeOffset date1 = new DateTime(filtry.DataOd.Year, filtry.DataOd.Month, filtry.DataOd.Day, 0, 0, 0);
                DateTimeOffset date2 = new DateTime(filtry.DataDo.Year, filtry.DataDo.Month, filtry.DataDo.Day, 23, 59, 59);

                string dataod = $"{date1:yyyy-MM-dd HH:mm:ss}";
                string datado = $"{date2:yyyy-MM-dd HH:mm:ss}";

                daty = daty.Replace("dataod", dataod).Replace("datado", datado);
            }



            return daty + statusy + platnosc_info;

        }




        async Task<List<SendOrder>> SendOrders()
        {

            string tmp = $@"cdn.PC_WykonajSelect N'select * from cdn.pc_ordernag '";

            var wynikii = await App.TodoManager.GetOrdersFromWeb(tmp);

            return wynikii;
        }



        List<SendOrder> sendOrders;

        public  void PobierzListe()
        {

            sendOrders = new List<SendOrder>();
            string _filtr = UstawFiltry();


            //await _connection.CreateTableAsync<OrderComplete>();

            //var wynik = await _connection.Table<OrderComplete>().ToListAsync();



            


            try
            {
                var wynik = Task.Run(() => SendOrders()).Result;
                //string tmp = "cdn.PC_WykonajSelect N'select * from cdn.pc_ordernag '";

                //var wynik = await App.TodoManager.GetOrdersFromWeb(tmp);

                GetOrders.Clear();

                connection.Open();
                MySqlCommand command1 = connection.CreateCommand();

                command1.CommandText = $@"SELECT *, zamowienia.id as zid, zamowienia.nr_paragonu as nr_paragonu  FROM zamowienia 
                            LEFT JOIN zamowienia_klienci ON zamowienia.zamowienie_klient_id = zamowienia_klienci.id 
                            where {_filtr}
                            ORDER BY zamowienia.id DESC  ";
                MySqlDataReader reader = command1.ExecuteReader();


                while (reader.Read())
                {
                    double num;
                    DateTime dateTime = Convert.ToDateTime(reader["data"]);
                    var _isFinish = (wynik.Where(s => s.Orn_OrderId == reader.GetInt32("zid") && s.Orn_IsDone == true)).Any();
                    Order item = new Order
                    {
                        //IsFinish = (wynik.Where(s => s.IdOrder == reader.GetInt32("zid"))).Any(),
                        IsEdit= (wynik.Where(s => s.Orn_OrderId == reader.GetInt32("zid") && s.Orn_IsEdit == true)).Any(),
                        IsFinish = (wynik.Where(s => s.Orn_OrderId== reader.GetInt32("zid") && s.Orn_IsDone==true)).Any(),
                        id = reader.GetInt32("zid"),
                        data = dateTime.ToString("yyyy-MM-dd HH:MM:ss"),
                        status = reader["status"].ToString(),
                        wartosc_netto = Math.Round(reader.GetDouble("wartosc_netto"), 2),
                        wysylka_koszt = reader.GetDouble("wysylka_koszt"),
                        platnosc_koszt = reader.GetDouble("platnosc_koszt"),
                        do_zaplaty = reader.GetDouble("do_zaplaty") - (double.TryParse(reader["korekta"].ToString(), out num) ? reader.GetDouble("korekta") : 0.0),
                        wysylka_info = reader["wysylka_info"].ToString(),
                        platnosc_info = reader["platnosc_info"].ToString(),
                        uwagi = reader["uwagi"].ToString(),
                        faktura_adres = reader["faktura_adres"].ToString(),
                        nr_paragonu = reader["nr_paragonu"].ToString(),
                        faktura_firma = reader["faktura_firma"].ToString(),
                        typ_platnosci = reader["typ_platnosci"].ToString(),
                        platnosc_karta_podarunkowa = reader.GetDouble("platnosc_karta_podarunkowa")
                    };
                    if (double.TryParse(reader["platnosc_punktami"].ToString(), out num))
                    {
                        item.platnosc_punktami = double.Parse(reader["platnosc_punktami"].ToString());
                    }

                    GetOrders.Add(item);
                }

                //connection.Close();
                //if(OrderList !=null)
                //IleZam = $"Lista zamówień ({OrderList.Count})";

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                
            }
            if (GetOrders != null)
                IleZam = $"Lista zamówień ({GetOrders.Count})";
            connection.Close();

        }

        
    }
}
