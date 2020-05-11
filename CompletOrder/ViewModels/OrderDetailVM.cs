using CompletOrder.Models;
using MySql.Data.MySqlClient;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CompletOrder.ViewModels
{
    public class OrderDetailVM : DataBaseConn, INotifyPropertyChanged
    {
        
        private SQLiteAsyncConnection _connection;
        
        public ObservableCollection<OrderDetail> orderDetail { get; set; }

        public Order _order = new Order();

        //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        
        ICommand showOtherLocation;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ShowOtherLocation
        {
            get { return showOtherLocation; }
        }
        public int _orderid { get; set; }
        public decimal SumaZamowienia { get; set; }
        public int PozycjiZamowienia{ get; set; }

        public OrderDetailVM(Order _order)
        {
            //_orderDetail = new OrderDetail();

            orderDetail = new ObservableCollection<OrderDetail>();
            //orderDetail =  _orderDetail.OrderDetailVM;
            //orderDetail=_orderDetail.OrderDetailVM =new ObservableCollection<OrderDetail>();

            this._order = _order;
            SumaZamowienia = (decimal)_order.do_zaplaty;
            _orderid = _order.id;
            orderDetail.OrderBy(x => x.IsDone);
            _connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();
            _connection.CreateTableAsync<OrderDatailComplete>();

            showOtherLocation = new Command(ShowOther);

            GetOrderDetail(_order.id);
            //pobierz();
        }


        void ShowOther(object s)
        {
            List<string> nowy = new List<string>();
            var stwrkarty = Task.Run(() => GetTwrKartyAsync());

            foreach (var wpis in stwrkarty.Result)
            {
                nowy.Add(String.Concat(wpis.Polozenie," : ", wpis.TwrStan, "szt"));
            }

            Application.Current.MainPage.DisplayActionSheet("Wszystkie położenia:", "OK", null, nowy.ToArray());

        }



        void pobierz()
        {
            var stwrkarty = Task.Run(() => GetTwrKartyAsync()).Result[0];


        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(backingField, value))
        //        return;

        //    backingField = value;

        //    OnPropertyChanged(propertyName);
        //}

        //private bool _isDone;
        //public bool IsDone
        //{
        //    get { return _isDone; }
        //    set
        //    {
        //        if (_isDone == value)
        //            return;

        //        _isDone = value;

        //        SetValue(ref _isDone, value);
        //        //OnPropertyChanged();
        //        OnPropertyChanged(nameof(Color));

        //    }
        //}


        //public Color Color
        //{
        //    get { return  IsDone? Color.Pink : Color.Black; }
        //}

        public static ObservableCollection<T> Convert2<T>(IList<T> original)
        {
            return new ObservableCollection<T>(original);
        }


        


        //List<OrderDatailComplete> orderDatailCompletes;


        async Task<List<OrderDatailComplete>> listaUkonczonych(int orderId)
        {

            var sss = await _connection.Table<OrderDatailComplete>().Where(c => c.IdOrder == orderId).ToListAsync();

           // await Task.Delay(500);
            return sss;
        }



        public  void GetOrderDetail(int orderId)
        {
            

            var wynik = Task.Run(() => listaUkonczonych(orderId)).Result; //dzoala

            //var wynik = await _connection.Table<OrderDatailComplete>().Where(c => c.IdOrder == orderId).ToListAsync(); ;

                DataTable dt = new DataTable();
            try
            {
                orderDetail.Clear();
                base.mysqlconn.Open();
                MySqlCommand command1 = base.mysqlconn.CreateCommand();
                command1.CommandText = "SELECT zamowienia_produkty.id IdElement,nr_katalogowy, ilosc, cena_netto, zamowienia_produkty.vat as zvat, promo, ilosc_zwrocona, nazwa " +
                    "FROM zamowienia_produkty LEFT JOIN produkty ON zamowienia_produkty.produkt_id=produkty.id WHERE main_id=" + orderId;
                MySqlDataReader reader = command1.ExecuteReader();
                
                while (reader.Read())
                {
                    PozycjiZamowienia++;
                    if (!string.IsNullOrEmpty(reader["nr_katalogowy"].ToString()))
                    {
                        var stwrkarty = Task.Run(() => GetTwrKartyAsync(reader["nr_katalogowy"].ToString())).Result[0] as TwrKarty;
                        var id_element = reader.GetInt32("IdElement");
                        // var tmp2 = (wynik.Where(s => s.IdOrder == orderId && s.IdElementOrder == id_element)).Any();
                        OrderDetail item = new OrderDetail
                        {
                            IsDone = (wynik.Where(s => s.IdOrder == orderId && s.IdElementOrder == id_element)).Any(),
                            OrderId = orderId,
                            IdElement = reader.GetInt32("IdElement"),
                            kod = reader["nr_katalogowy"].ToString(),
                            //twrkarty = GetTwrInfo(reader["nr_katalogowy"].ToString()),
                            twrkarty = stwrkarty,//GetTwrKartyAsync(reader["nr_katalogowy"].ToString()).Result[0] as TwrKarty,
                            promo = reader["promo"].ToString().Replace("&quot;", "").Replace("_", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", ""),
                            nazwa = reader["nazwa"].ToString().Replace(reader["nr_katalogowy"].ToString(), ""),
                            ilosc = Convert.ToInt32(reader["ilosc"]) - Convert.ToInt32(reader["ilosc_zwrocona"]),
                            cena_netto = reader.GetDouble("cena_netto"),
                            vat = reader.GetInt32("zvat")
                        };
                        if (item.ilosc > 0)
                        {
                            orderDetail.Add(item);
                        }
                        var tmp = orderDetail.OrderBy(x => x.IsDone).ThenBy(x => x.twrkarty.MgA_Segment1).ThenBy(x => x.twrkarty.MgA_Segment2).ThenBy(x => x.twrkarty.MgA_Segment3);
                        orderDetail = Convert2(tmp.ToList());

                    }
                    
                }
              
                base.mysqlconn.Close();
            }
            catch (Exception )
            {

                
            }   
            
        }



        public async Task<IList<TwrKarty>> GetTwrKartyAsync(string _twrkod= "112WMWHT1711")
        {

            string Webquery = $@"cdn.PC_WykonajSelect N' select twr_kod TwrKod, TZM_TPaId, CDN.ifs_PodajPolozenie(mga_id) Polozenie,MgA_Segment1,
MgA_Segment2,MgA_Segment3, replace(twr_url,twr_kod,''miniatury/''+twr_kod) TwrUrl, cast(tzm_ilosc as int) as TwrStan
                    FROM cdn.twrkarty
                    left join CDN.TwrPartie on tpa_twrnumer = twr_gidnumer
                    LEFT JOIN CDN.TwrZasobyMag ON CDN.TwrPartie.TPa_Id = CDN.TwrZasobyMag.TZM_TPaId AND CDN.TwrZasobyMag.TZM_MagNumer = 41 
                    LEFT JOIN CDN.MagAdresy ON CDN.TwrZasobyMag.TZM_MgAId = CDN.MagAdresy.MgA_Id 
                    
                    WHERE Twr_kod = ''{_twrkod}'' order by tzm_ilosc desc'"; 

            var movies = await App.TodoManager.GetDataFromWeb(Webquery);

            return movies;

        }



        //TwrKarty GetTwrInfo(string _twrkod) 
        //{

        //    var twrkarty = new TwrKarty();
            
        //    var querystring = $@"SELECT twr_kod TwrKod, TZM_TPaId, CDN.ifs_PodajPolozenie(mga_id) Polozenie,MgA_Segment1,MgA_Segment2,MgA_Segment3, replace(twr_url,twr_kod,'miniatury/'+twr_kod) TwrUrl
        //            FROM cdn.twrkarty
        //            left join CDN.TwrPartie on tpa_twrnumer = twr_gidnumer
        //            LEFT JOIN CDN.TwrZasobyMag ON CDN.TwrPartie.TPa_Id = CDN.TwrZasobyMag.TZM_TPaId AND CDN.TwrZasobyMag.TZM_MagNumer = 41 
        //            LEFT JOIN CDN.MagAdresy ON CDN.TwrZasobyMag.TZM_MgAId = CDN.MagAdresy.MgA_Id 
        //            LEFT JOIN CDN.MagObszary ON CDN.MagAdresy.MgA_MgOId = CDN.MagObszary.MgO_Id 
        //            WHERE Twr_kod = '{_twrkod}' ";

        //    using (SqlConnection connection = new SqlConnection(sqlconn))
        //    {
        //        connection.Open();
        //        using (SqlCommand command2 = new SqlCommand(querystring, connection))
        //        using (SqlDataReader reader = command2.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {

        //                int _MgA_Segment1;
        //                int _MgA_Segment2;
        //                int _MgA_Segment3;


        //                bool tak1= Int32.TryParse(reader.GetString(reader.GetOrdinal("MgA_Segment1")), out _MgA_Segment1);
        //                bool tak2= Int32.TryParse(reader.GetString(reader.GetOrdinal("MgA_Segment2")), out _MgA_Segment2);
        //                bool tak3= Int32.TryParse(reader.GetString(reader.GetOrdinal("MgA_Segment3")), out _MgA_Segment3);
        //                TwrKarty _twrKarty = new TwrKarty
        //                {
        //                    TwrKod = reader.GetString(reader.GetOrdinal("TwrKod")),
        //                    Polozenie = reader.GetString(reader.GetOrdinal("Polozenie")),
        //                    TwrUrl = reader.GetString(reader.GetOrdinal("TwrUrl")),
        //                    //MgA_Segment1 = tak1 ? _MgA_Segment1 : 0,
        //                    //MgA_Segment2 = tak2 ? _MgA_Segment2 : 0,
        //                    //MgA_Segment3 = tak3 ? _MgA_Segment3 : 0 
                            
        //                };
        //                twrkarty = _twrKarty;
        //            }
        //        }
        //    }
        //    return twrkarty;
        //}

    }
}
