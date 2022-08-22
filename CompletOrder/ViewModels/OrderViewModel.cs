using CompletOrder.Models;
using CompletOrder.Views;
 
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
using CompletOrder.Services;
using MySqlConnector;

namespace CompletOrder.ViewModels
{
    public class OrderViewModel :DataBaseConn, INotifyPropertyChanged
    {
        private MySqlConnection connection;
        //private SQLiteAsyncConnection _connection;

        //public ObservableCollection<Order> OrderList { get; set; }
        PrestaWeb prestaWeb;
        private ObservableCollection<Order> _orderList;
        private ObservableCollection<Allegro> _allegroList;
       // private ObservableCollection<Order> GetOrders;
        private ObservableCollection<Presta> _prestaNagList;

        public ICommand SaveConnectionSettings { private set; get; }

        public ObservableCollection<Presta> PrestaNagList
        {
            get { return _prestaNagList; }
            set
            {                
                SetValue(ref _prestaNagList, value);                
            }
        }


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
        public ObservableCollection<Allegro> AllegroList
        {
            get { return _allegroList; }
            set
            {
                //_orderList = value;
                SetValue(ref _allegroList, value);
                //OnPropertyChanged(nameof(OrderList));
            }
        }


         

        private string _nazwaPlatnosci;

        public string NazwaPlatnosci
        {
            get { return _nazwaPlatnosci; }
            set 
            {
                SetValue(ref _nazwaPlatnosci, value);
                //PobierzListe();
            }
        }

    
        public string Passwordsql
        {
            get { return passwordsql; }
            set
            {
                
                SetValue( ref passwordsql, value);
                OnPropertyChanged(nameof(Passwordsql));
                //Save();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public OrderViewModel()
        {
            //OrderList = new ObservableCollection<Order>();
            //GetOrders = new ObservableCollection<Order>(); 
            AllegroList = new ObservableCollection<Allegro>(); 
            _prestaNagList = new ObservableCollection<Presta>(); 

            connection = new MySqlConnection(conn_string.ToString());

            //_connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();

            //PobierzListe();
            //PobierzAllegro();
            prestaWeb = new PrestaWeb();

            wynik = new List<SendOrder>();
            //PobierzListeZatwierdzonychZamowien();
            // przeniosłem z allegro i pobierz liste
            SaveConnectionSettings = new Command(async () =>  await ExecuteSaveConnection());



            //if (GetOrders !=null)
            //OrderList = GetOrders;
        }

        private async Task ExecuteSaveConnection()
        {
              await Task.Run(() => Connect());
            await App.Current.MainPage.Navigation.PopAsync();
        }

        public void PobierzListeZatwierdzonychZamowien()
        {
            wynik = Task.Run(() => SendOrders()).Result;

            if (PrestaNagList.Count > 0)
                foreach (var ss in PrestaNagList)
                {
                    //ss.IsFinish = (wynik.Where(s => s.Orn_OrderId == ss.ZaN_GIDNumer )).Any();//&& s.Orn_IsDone == true


                    ss.IsFinish = ( 
                                  from nowa in wynik                                        
                                  where nowa.Orn_OrderId ==ss.ZaN_GIDNumer
                                  select   nowa.Orn_IsDone).SingleOrDefault(); 

                }
        }
             
 
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


           // OnPropertyChanged(propertyName);
        }


        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetValue(ref isBusy, value); }
        }


        private string _ileZam;
        private string _ileZam2;

        public string IleZam
        {
            get { return _ileZam; }
            set
            {
                SetValue(ref _ileZam, value);
                OnPropertyChanged(nameof(IleZam));
            }
        }


        public string IleZam2
        {
            get { return _ileZam2; }
            set
            {
                SetValue(ref _ileZam2, value);
                OnPropertyChanged(nameof(IleZam2));
            }
        }

        //public string Filter
        //{
        //    get { return _filter; }
        //    set
        //    {
        //        SetValue(ref _filter, value);
        //        Search();
        //    }
        //}


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
                    //PobierzListe();
                    UstawFiltry();
                    //OrderList = GetOrders;
                }
                else { 
                
                   // PobierzListe();
                   // OrderList = GetOrders;
                }
            }
        }

        public static ObservableCollection<T> Convert2<T>(IList<T> original)
        {
            return new ObservableCollection<T>(original);
        }


        //public ICommand SearchCommand => new Command(Search);
        //public void Search()
        //{
        //    if (string.IsNullOrWhiteSpace(_filter))
        //    {
        //        OrderList = GetOrders;// ItemData.Items;
        //    }
        //    else
        //    {
        //        var tmp = GetOrders.Where(c => c.id.ToString().Contains(_filter)).ToList();
        //        OrderList = Convert2(tmp);
        //    }
              
        //}


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

                if (filtry.OnlyToDoKompletacja)
                    statusy += "'zapłacone',";

                if (filtry.SortASC)
                    statusy += "'zrealizowane',";


                if (statusy == "status in (")
                {
                    statusy = "";
                }
                else {
                    statusy += ")";
                    statusy = statusy.Replace(",)", ")");
                }
                

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

            var platnosc_string = platnosc_info == " AND platnosc_info LIKE '%xxxx%'" ? "" : platnosc_info;

            var returnString = (daty + statusy + platnosc_string) == ""?"":"where "+ daty + statusy + platnosc_string;

            return returnString;

        }


        public void PobierzAllegro()
        {

            if (IsBusy)
                return;

            IsBusy = true;

            AllegroList.Clear();
            var tmp = Task.Run(() => GetAllegros()).Result;
           // wynik = Task.Run(() => SendOrders()).Result;

            //var nowa = (
            //    from allegro in tmp
            //    join done in wynik on allegro.Id equals done.Orn_OrderId into pod
            //    from ales in pod.DefaultIfEmpty()
            //    select ales
            //    );


            foreach (var a in tmp)
            {
                AllegroList.Add(new Allegro
                {
                    Id = a.Id,
                    ElementId = a.ElementId,
                    CustomerName = a.CustomerName,
                    forma_platnosc = a.forma_platnosc,
                    ilosc = a.ilosc,
                    IsFinish = (wynik.Where(s => s.Orn_OrderId == a.Id && s.Orn_IsDone == true)).Any(),
                    kod = a.kod,
                    nazwa = a.nazwa,
                    NrParagonu = a.NrParagonu,
                    Pol1 = a.Pol1,
                    Pol2 = a.Pol2,
                    Pol3 = a.Pol3,
                    RaportDate = a.RaportDate,
                    typ_wysylka=a.typ_wysylka

                });
            }
             
            IleZam2 = $"Lista zamówień ({AllegroList.Count})";
            IsBusy = false;
        }

        async Task<ObservableCollection<Allegro>> GetAllegros()
        {
           
            
            string filtr = "";

            if (!string.IsNullOrEmpty(SettingsPage.SendMetod))
            {
                if(SettingsPage.SendMetod != "!Wszystkie")
                
                
                filtr = $"where typ_wysylka=''{SettingsPage.SendMetod}''";
            }


            try
            {
                string tmp = $@"cdn.PC_WykonajSelect N' select distinct   Id, CustomerName, RaportDate,    forma_platnosc,  typ_wysylka 
                            from cdn.pc_allegroorders  {filtr}  order by RaportDate asc'";

                var wynikii = await App.TodoManager.GetOrdersFromAllegro(tmp);

                return wynikii;
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        async Task<List<SendOrder>> SendOrders()
        {
            var filtry = Application.Current as App;
            DateTimeOffset dateOd = new DateTime(filtry.DataOd.Year, filtry.DataOd.Month, filtry.DataOd.Day, 0, 0, 0);
            DateTimeOffset dateDo = new DateTime(filtry.DataDo.Year, filtry.DataDo.Month, filtry.DataDo.Day, 23, 59, 59);
            var sort = filtry.SortASC ? "ASC" : "DESC";
            var dataod = dateOd.ToString("yyyy-MM-dd HH:mm:ss");
            var datado = dateDo.ToString("yyyy-MM-dd HH:mm:ss");


            string tmp = $@"cdn.PC_WykonajSelect N'select * from cdn.pc_ordernag where  Orn_OrderData>=''{dataod}'''";

            var wynikii = await App.TodoManager.GetOrdersFromWeb(tmp);

            return wynikii;
        }



        List<SendOrder> sendOrders;
        private List<SendOrder> wynik;
        private string passwordsql;

        //public  void PobierzListe()
        //{

        //    sendOrders = new List<SendOrder>();
        //    string _filtr = UstawFiltry(); 

        //    if(_filtr!= "where data between 'dataod' and 'datado' and status in (")
        //    {

        //        try
        //        {
        //            //var wynik = Task.Run(() => SendOrders()).Result;  // przeniesione do konstruktra

        //            //string tmp = "cdn.PC_WykonajSelect N'select * from cdn.pc_ordernag '";

        //            //var wynik = await App.TodoManager.GetOrdersFromWeb(tmp);

        //           // GetOrders.Clear();

        //            connection.Open();
        //            MySqlCommand command1 = connection.CreateCommand();

        //            command1.CommandText = $@"SELECT *, zamowienia.id as zid, zamowienia.nr_paragonu as nr_paragonu  FROM zamowienia 
        //                    LEFT JOIN zamowienia_klienci ON zamowienia.zamowienie_klient_id = zamowienia_klienci.id 
        //                    {_filtr}
        //            ORDER BY zamowienia.id DESC  ";
        //            MySqlDataReader reader = command1.ExecuteReader();


        //            while (reader.Read())
        //            {
        //                double num;
        //                DateTime dateTime = Convert.ToDateTime(reader["data"]);
        //                var _isFinish = (wynik.Where(s => s.Orn_OrderId == reader.GetInt32("zid") && s.Orn_IsDone == true)).Any();
        //                Order item = new Order
        //                {
        //                    //IsFinish = (wynik.Where(s => s.IdOrder == reader.GetInt32("zid"))).Any(),
        //                    IsEdit = (wynik.Where(s => s.Orn_OrderId == reader.GetInt32("zid") && s.Orn_IsEdit == true)).Any(),
        //                    IsFinish = (wynik.Where(s => s.Orn_OrderId == reader.GetInt32("zid") && s.Orn_IsDone == true)).Any(),
        //                    id = reader.GetInt32("zid"),
        //                    data = dateTime.ToString("yyyy-MM-dd HH:MM:ss"),
        //                    status = reader["status"].ToString(),
        //                    wartosc_netto = Math.Round(reader.GetDouble("wartosc_netto"), 2),
        //                    wysylka_koszt = reader.GetDouble("wysylka_koszt"),
        //                    platnosc_koszt = reader.GetDouble("platnosc_koszt"),
        //                    do_zaplaty = reader.GetDouble("do_zaplaty") - (double.TryParse(reader["korekta"].ToString(), out num) ? reader.GetDouble("korekta") : 0.0),
        //                    wysylka_info = reader["wysylka_info"].ToString(),
        //                    platnosc_info = reader["platnosc_info"].ToString(),
        //                    uwagi = reader["uwagi"].ToString(),
        //                    faktura_adres = reader["faktura_adres"].ToString(),
        //                    nr_paragonu = reader["nr_paragonu"].ToString(),
        //                    faktura_firma = reader["faktura_firma"].ToString(),
        //                    typ_platnosci = reader["typ_platnosci"].ToString(),
        //                    platnosc_karta_podarunkowa = reader.GetDouble("platnosc_karta_podarunkowa")
        //                };
        //                if (double.TryParse(reader["platnosc_punktami"].ToString(), out num))
        //                {
        //                    item.platnosc_punktami = double.Parse(reader["platnosc_punktami"].ToString());
        //                }

        //                GetOrders.Add(item);
        //            }

        //            //connection.Close();
        //            //if(OrderList !=null)
        //            //IleZam = $"Lista zamówień ({OrderList.Count})";

        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex);

        //        }
        //        if (GetOrders != null)
        //            IleZam = $"Lista zamówień ({GetOrders.Count})";
        //        connection.Close();
        //    }

        //}



        public void GetPrestaZam()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            var app = Application.Current as App;

            //if ( !app.OnlyToDoKompletacja)
            
                PrestaNagList.Clear();

                
               //if(PrestaNagList.Count==0)
                  PrestaNagList = Task.Run(() => prestaWeb.PobierzZamówieniaSql()).Result;



                if (PrestaNagList.Count > 0)
                    foreach (var ss in PrestaNagList)
                    {
                        ss.IsFinish = (wynik.Where(s => s.Orn_OrderId == ss.ZaN_GIDNumer && s.Orn_IsDone == true)).Any();

                    }
                IsBusy = false;
                
            
            
            //else if (app.OnlyToDoKompletacja && PrestaNagList.Count > 0) 
            //{
            //    foreach (var item in PrestaNagList.ToList())
            //    {

            //        PrestaNagList.Remove(PrestaNagList.SingleOrDefault(i => i.IsFinish == true && i.ZaN_GIDNumer == item.ZaN_GIDNumer));
            //    }
            //    IsBusy = false;
            //}
            

                if (app.SortASC)
                {

                    PrestaNagList.OrderBy(s => s.ZaN_GIDNumer);
                }
                else
                    PrestaNagList.OrderByDescending(s => s.ZaN_GIDNumer);


            var suma = Decimal.Round(PrestaNagList.Sum(s => s.WartoscZam)/1000,1,MidpointRounding.AwayFromZero);

            IleZam = $"Lista zamówień ({PrestaNagList.Count}), {suma}k"; 
             

            IsBusy = false;

        }




        public void GetPrestaZam(bool onlyRefresh)
        {

            if (IsBusy)
                return;

            IsBusy = true;
                var app = Application.Current as App;

            if (!onlyRefresh &&!app.OnlyToDoKompletacja)
            {
                PrestaNagList.Clear();

                var querystring = $@" cdn.PC_WykonajSelect N'
                    select   
	                    ZaN_GIDNumer, 
	                    ZaN_FormaNazwa, 
	                    ZaN_DokumentObcy, 
	                    ZaN_SpDostawy, 
	                    cast(dateadd(d,ZaN_DataWystawienia,''18001228'') as date)ZaN_DataWystawienia,
	                    cast(dateadd(d,ZaN_DataRealizacji,''18001228'') as date)ZaN_DataRealizacji, 
	                    ZaN_Stan,
	                    knt.KnA_Akronim,
	                    sum(ele.ZaE_WartoscPoRabacie)WartoscZam
                    from cdn.ZamNag
	                      join cdn.KntAdresy knt on KnA_GIDTyp=ZaN_KnATyp AND KnA_GIDNumer=ZaN_KnANumer  
	                      left join cdn.ZamElem ele on ZaN_GIDNumer=ZaE_GIDNumer
                      where ZaN_GIDTyp=960
                      group by ZaN_GIDNumer, 
	                    ZaN_FormaNazwa, 
	                    ZaN_DokumentObcy, 
	                    ZaN_SpDostawy,ZaN_DataWystawienia,ZaN_DataRealizacji,ZaN_Stan,knt.KnA_Akronim
                '"; //queery to sql

                //_prestaNagList= await App.TodoManager.GetOrdersFromPresta(querystring);

                PrestaNagList = Task.Run(() => prestaWeb.PobierzZamówieniaSql()).Result;  
                
                IsBusy = false;

            }
            
            else if(app.OnlyToDoKompletacja &&PrestaNagList.Count>0)//tylko nieskompletowane
            {
                foreach (var item in PrestaNagList.ToList())
                {

                 PrestaNagList.Remove(PrestaNagList.SingleOrDefault(i => i.IsFinish ==true && i.ZaN_GIDNumer==item.ZaN_GIDNumer));
                }
                IsBusy = false;
            }
            else if (onlyRefresh)
            {

                if (app.SortASC)
                {

                    var dsdsa=PrestaNagList.OrderBy(s => s.ZaN_GIDNumer).ToList();
                }
                else
                    PrestaNagList.OrderByDescending(s => s.ZaN_GIDNumer);
                IsBusy = false;
                return;
            }

            //if (PrestaNagList.Count > 0)
            //    foreach (var ss in PrestaNagList)
            //    {
            //        ss.IsFinish = (wynik.Where(s => s.Orn_OrderId == ss.ZaN_GIDNumer && s.Orn_IsDone == true)).Any();

            //    }

            IleZam = $"Lista zamówień ({PrestaNagList.Count})";

            #region sql

            //PrestaNagList = await prestaWeb.PobierzZamówienia();

            //PrestaNagList = _prestaNagList;

            //using (SqlConnection connection = new SqlConnection(sqlconn))
            //{
            //    connection.Open();
            //    using (SqlCommand command2 = new SqlCommand(querystring, connection))
            //    using (SqlDataReader reader = command2.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            _prestaNagList.Add(new Presta
            //            {

            //                ZaN_GIDNumer = Convert.ToInt32(reader["ZaN_GIDNumer"]),
            //                ZaN_FormaNazwa = reader["ZaN_FormaNazwa"].ToString(),
            //                ZaN_DataRealizacji = reader["ZaN_DataRealizacji"].ToString(),
            //                ZaN_DataWystawienia = reader["ZaN_DataWystawienia"].ToString(),
            //                ZaN_DokumentObcy = reader["ZaN_DokumentObcy"].ToString(),
            //                ZaN_SpDostawy = reader["ZaN_SpDostawy"].ToString(),
            //                ZaN_Stan = reader["ZaN_Stan"].ToString(),
            //                WartoscZam = Convert.ToDecimal(reader["WartoscZam"]),
            //                KnA_Akronim = reader["KnA_Akronim"].ToString(),
            //                IsFinish= (wynik.Where(s => s.Orn_OrderId == Convert.ToInt32(reader["ZaN_GIDNumer"]) && s.Orn_IsDone == true)).Any()
            //            });

            //        }
            //    }
            //} 
            #endregion

            IsBusy = false;

        }


    }
}
