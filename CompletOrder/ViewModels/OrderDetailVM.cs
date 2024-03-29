﻿using CompletOrder.Models;
 
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
using CompletOrder.Services;
using System.Globalization;
using System.Runtime.CompilerServices;
using MySqlConnector;

namespace CompletOrder.ViewModels
{
    public class OrderDetailVM : DataBaseConn//, INotifyPropertyChanged
    {

        private SQLiteAsyncConnection _connection;


        public ObservableCollection<OrderDetail> OrderDetail { get; set; }
        //public ObservableCollection<OrderDetail> OrderDetail;

        //public ObservableCollection<OrderDetail> OrderDetail
        //{
        //    get { return orderDetail; }
        //    set
        //    {
        //        { SetValue(ref orderDetail, value); }
        //    }
        //}



        private PrestaWeb prestaWeb;

        public ObservableCollection<Allegro> AllegroList { get; set; }
        public ObservableCollection<Presta> PrestaElemList { get; set; }

        public Order _order = new Order();



        ICommand showOtherLocation;

        
        public ICommand ShowOtherLocation
        {
            get { return showOtherLocation; }
        }
        public int _orderid { get; set; }
        public decimal SumaZamowienia { get; set; }
        public int PozycjiZamowienia { get; set; }
        public int SztukZamówienia { get; set; }

        public OrderDetailVM(Order _order)
        {      

            OrderDetail = new ObservableCollection<OrderDetail>();
 

            this._order = _order;
            SumaZamowienia = (decimal)_order.do_zaplaty;
            _orderid = _order.id;
            OrderDetail.OrderBy(x => x.IsDone);
            _connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();
            _connection.CreateTableAsync<OrderDatailComplete>();

            showOtherLocation = new Command(ShowOther);

            GetOrderDetail(_order.id);
            //pobierz();
        }

        public OrderDetailVM(Allegro allegro)
        {

            _connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();
            _connection.CreateTableAsync<OrderDatailComplete>();

            OrderDetail = new ObservableCollection<OrderDetail>();

            AllegroList = Task.Run(() => GetAllegros(allegro.Id)).Result;

            _orderid = allegro.Id;
            //var wynik = Task.Run(() => listaUkonczonych(allegro.Id)).Result;
            var wynik = Task.Run(() => listaUkonczonychSQL(allegro.Id)).Result;

            foreach (var a in AllegroList)
            {
                PozycjiZamowienia++;

                if (a.kod == "331ZKWBDM451-027")
                    a.kod = "331LKWBDM451-027";


                var stwrkarty = Task.Run(() => GetTwrKartyAsync(a.kod)).Result;
                if (stwrkarty.Count > 0)
                {
                    OrderDetail.Add(new OrderDetail
                    {
                        OrderId = a.Id,
                        ilosc = a.ilosc,
                        nazwaShort = a.nazwa,
                        kod = a.kod,
                        twrkarty = stwrkarty[0],
                        //IsDone = (wynik.Where(s => s.IdOrder == _orderid && s.IdElementOrder == a.ElementId)).Any(),
                        IsDone = (wynik.Where(s => s.OrE_OrderId == _orderid && s.OrE_OrderEleId == a.ElementId)).Any(),
                        IdElement = a.ElementId,
                        nazwa = a.nazwa
                         

                    });
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("uwaga", $"nie pobrano danych dla \n {a.kod}", "OK");
                }
            }

            var tmp = OrderDetail.OrderBy(x => x.IsDone).ThenBy(x => x.twrkarty.MgA_Segment1).ThenBy(x => x.twrkarty.MgA_Segment2).ThenBy(x => x.twrkarty.MgA_Segment3);
            OrderDetail = Convert2(tmp.ToList());
            //orderDetail.OrderBy(x => x.IsDone);
        }

        public OrderDetailVM(Presta presta)
        {
            try
            {
                _connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();
                _connection.CreateTableAsync<OrderDatailComplete>();

                OrderDetail = new ObservableCollection<OrderDetail>();
                prestaWeb = new PrestaWeb();
                PrestaElemList = Task.Run(() => GetPrestaElem(presta.ZaN_GIDNumer)).Result;
                

                _orderid = presta.ZaN_GIDNumer;
                //var wynik = Task.Run(() => listaUkonczonych(presta.ZaN_GIDNumer)).Result;
                var wynik = Task.Run(() => listaUkonczonychSQL(presta.ZaN_GIDNumer)).Result;
                var nazwakrtka = "";
                foreach (var a in PrestaElemList)
                {
                    if (a.ZaE_TwrNazwa.IndexOf("- Kolor") > 0)
                    {

                        nazwakrtka = a.ZaE_TwrNazwa.Substring(0, a.ZaE_TwrNazwa.IndexOf("- Kolor") - 1);
                    }
                    else if (a.ZaE_TwrNazwa.IndexOf("- Rozmiar") > 0)
                    {
                        nazwakrtka = a.ZaE_TwrNazwa.Substring(0, a.ZaE_TwrNazwa.IndexOf("- Rozmiar") - 1);

                    }
                    else
                    {
                        nazwakrtka = a.ZaE_TwrNazwa;
                    }

                    //TODO : jak zły kod to wywala

                    PozycjiZamowienia++;



                    var TwrKarty = Task.Run(() => GetTwrKartyAsync(a.ZaE_TwrKod)).Result;

                    if (TwrKarty.Count > 0)
                    {
                        OrderDetail.Add(new OrderDetail
                        {
                            OrderId = a.ZaN_GIDNumer,
                            ilosc = a.ZaE_Ilosc,
                            nazwa = a.ZaE_TwrNazwa,
                            kod = a.ZaE_TwrKod,
                            twrkarty = TwrKarty[0],
                            //IsDone = (wynik.Where(s => s.IdOrder == _orderid && s.IdElementOrder == a.ElementId)).Any(),
                            IsDone = (wynik.Where(s => s.OrE_OrderId == _orderid && s.OrE_OrderEleId == a.ElementId)).Any(),
                            IdElement = a.ElementId,
                            cena_netto = System.Convert.ToDouble(a.WartoscZam),
                            kolor = a.Kolor,
                            rozmiar = a.Rozmiar,
                            nazwaShort = nazwakrtka
                        });
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("uwaga", $"nie pobrano danych dla \n {a.ZaE_TwrKod}", "OK");
                    }


                }
                var tmp = OrderDetail.OrderBy(x => x.IsDone).ThenBy(x => x.twrkarty.MgA_Segment1).ThenBy(x => x.twrkarty.MgA_Segment2).ThenBy(x => x.twrkarty.MgA_Segment3);
                OrderDetail = Convert2(tmp.ToList());
            }
            catch (Exception s)
            {

                var dsa = s.Message;
            }
            //orderDetail.OrderBy(x => x.IsDone);
        }


        void ShowOther(object s)
        {
            List<string> nowy = new List<string>();
            var stwrkarty = Task.Run(() => GetTwrKartyAsync());

            foreach (var wpis in stwrkarty.Result)
            {
                nowy.Add(String.Concat(wpis.Polozenie, " : ", wpis.TwrStan, "szt"));
            }

            Application.Current.MainPage.DisplayActionSheet("Wszystkie położenia:", "OK", null, nowy.ToArray());

        }



        public IList<TwrKarty> GetPolozeniaByTwr(string kodtwr)
        {
            return Task.Run(() => GetTwrKartyAsync(kodtwr)).Result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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



        //private TwrKarty twr;

        //public TwrKarty TwrKarty
        //{
        //    get { return twr; }
        //    set
        //    {
        //        { SetValue(ref twr, value); }
        //    }
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
            //var sss = await SelectOrderElem(orderId);


            return sss;
        }

        async Task<IList<PC_SiOrderElem>> listaUkonczonychSQL(int orderId)
        {

            //var sss = await _connection.Table<OrderDatailComplete>().Where(c => c.IdOrder == orderId).ToListAsync();
            var sss = await SelectOrderElem(orderId);


            return sss;
        }

        async Task<ObservableCollection<Allegro>> GetAllegros(int id)
        {

            string tmp = $@"cdn.PC_WykonajSelect N' select *
                            from cdn.pc_allegroorders where id={id}'";

            var wynikii = await App.TodoManager.GetOrdersFromAllegro(tmp);

            return wynikii;
        }

        async Task<ObservableCollection<Presta>> GetPrestaElem(int id)
        {

            ObservableCollection<Presta> _prestaNagList = new ObservableCollection<Presta>();
            try
            {
                //to nie jest uzywane
                string querystring = $@"cdn.PC_WykonajSelect N'     select   
	                    ZaN_GIDNumer, 
	                    ZaN_FormaNazwa, 
	                    ZaN_DokumentObcy,  
	                    knt.KnA_Akronim, 
						ZaE_Ilosc,
						ZaE_TwrNazwa,
						ZaE_TwrNumer,
						ZaE_WartoscPoRabacie WartoscZam,
						ZaE_TwrKod,
						concat(ZaE_GIDNumer,ZaE_GIDLp)ElementId
                    from cdn.ZamNag
	                      join cdn.KntAdresy knt on KnA_GIDTyp=ZaN_KnATyp AND KnA_GIDNumer=ZaN_KnANumer  
	                      left join cdn.ZamElem ele on ZaN_GIDNumer=ZaE_GIDNumer
                      where ZaN_GIDTyp=960
					  and ZaN_GIDNumer={id}'";

                // _prestaNagList = await App.TodoManager.GetOrdersFromPresta(querystring);
                //_prestaNagList = await Task.Run(() => prestaWeb.PobierzelementyZamówienia(id));
                _prestaNagList = await Task.Run(() => prestaWeb.MySqlPobierzelementyZamówienia(id));



                return _prestaNagList;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public void GetOrderDetail(int orderId)
        {


            //        var wynik = Task.Run(() => listaUkonczonych(orderId)).Result; //dzoala
            var wynik = Task.Run(() => listaUkonczonychSQL(orderId)).Result; //dzoala

            //var wynik = await _connection.Table<OrderDatailComplete>().Where(c => c.IdOrder == orderId).ToListAsync(); ;

            DataTable dt = new DataTable();
            try
            {
                OrderDetail.Clear();
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
                            IsDone = (wynik.Where(s => s.OrE_OrderId == orderId && s.OrE_OrderEleId == id_element)).Any(),
                            OrderId = orderId,
                            IdElement = reader.GetInt32("IdElement"),
                            kod = reader["nr_katalogowy"].ToString(),
                            //twrkarty = GetTwrInfo(reader["nr_katalogowy"].ToString()),
                            twrkarty = stwrkarty,//GetTwrKartyAsync(reader["nr_katalogowy"].ToString()).Result[0] as TwrKarty,
                            promo = reader["promo"].ToString().Replace("&quot;", "").Replace("_", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", ""),
                            nazwa = reader["nazwa"].ToString().Replace(reader["nr_katalogowy"].ToString(), ""),
                            ilosc = System.Convert.ToInt32(reader["ilosc"]) - System.Convert.ToInt32(reader["ilosc_zwrocona"]),
                            cena_netto = reader.GetDouble("cena_netto"),
                            vat = reader.GetInt32("zvat")
                        };
                        if (item.ilosc > 0)
                        {
                            OrderDetail.Add(item);
                        }
                        var tmp = OrderDetail.OrderBy(x => x.IsDone).ThenBy(x => x.twrkarty.MgA_Segment1).ThenBy(x => x.twrkarty.MgA_Segment2).ThenBy(x => x.twrkarty.MgA_Segment3);
                        OrderDetail = Convert2(tmp.ToList());

                    }

                }

                base.mysqlconn.Close();
            }
            catch (Exception)
            {


            }

        }




        internal async Task<bool> AddOrderElem(PC_SiOrderElem orderElem)
        {
            try
            {
                bool IsAddRow = true;

                {



                    var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var sqlInsert = $@"cdn.PC_WykonajSelect N'insert into cdn.PC_SiOrderElem values (
                                    {orderElem.OrE_OrderId},
                                    {orderElem.OrE_OrderEleId},
                                    {orderElem.OrE_Quantity},
                                    {orderElem.OrE_MpaId},
                                    ''{orderElem.OrE_PlaceName}'',
                                    null,
                                    ''{data}''
                                    )
                                if @@ROWCOUNT>0
                                                select ''OK'' as OrE_PlaceName
'";

                    var response = await App.TodoManager.PobierzDaneZWeb<PC_SiOrderElem>(sqlInsert);
                    if (response != null)
                    {
                        if (response.Count > 0)
                            return IsAddRow;
                    }

                }




                return IsAddRow = false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task<bool> DeleteOrderElem(OrderDatailComplete orderElem)
        {
            try
            {
                bool IsAddRow = true;

                {

                    var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var sqlInsert = $@"cdn.PC_WykonajSelect N'delete from cdn.PC_SiOrderElem 
                                    where OrE_OrderId={orderElem.IdOrder} and
                                    OrE_OrderEleId={orderElem.IdElementOrder}
                                     
                                    if @@ROWCOUNT>0
                                                select ''OK'' as OrE_PlaceName
                    '";

                    var response = await App.TodoManager.PobierzDaneZWeb<PC_SiOrderElem>(sqlInsert);
                    if (response != null)
                    {
                        if (response.Count > 0)
                            return IsAddRow;
                    }

                }

                return IsAddRow = false;
            }
            catch (Exception)
            {

                throw;
            }
        }


        internal async Task<IList<PC_SiOrderElem>> SelectOrderElem(int OrE_OrderId, int idElement = 0)
        {
            try
            {
                //bool IsAddRow = true;
                var addElement = idElement > 0 ? $" and OrE_OrderEleId={idElement}" : "";

                {

                    var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var sqlInsert = $@"cdn.PC_WykonajSelect N'select * from cdn.PC_SiOrderElem 
                                    where OrE_OrderId={OrE_OrderId}  {addElement} 
                                     
                                    
                    '";

                    //and OrE_OrderEleId={orderElem.OrE_OrderEleId}
                    return await App.TodoManager.PobierzDaneZWeb<PC_SiOrderElem>(sqlInsert);
                    //if (response != null)
                    //{
                    //    if (response.Count > 0)
                    //        return IsAddRow;
                    //}

                }

                //return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IList<TwrKarty>> GetTwrKartyAsync(string _twrkod = "112WMWHT1711")
        {

            string Webquery = $@"cdn.PC_WykonajSelect N' select twr_kod TwrKod, TZM_TPaId, CDN.ifs_PodajPolozenie(mga_id) Polozenie,MgA_Segment1,
                    MgA_Segment2,MgA_Segment3, cdn.pc_gettwrurl(twr_kod) as TwrUrl, cast(tzm_ilosc as int) as TwrStan, MgA_Id
                    FROM cdn.twrkarty
                    left join CDN.TwrPartie on tpa_twrnumer = twr_gidnumer
                    LEFT JOIN CDN.TwrZasobyMag ON CDN.TwrPartie.TPa_Id = CDN.TwrZasobyMag.TZM_TPaId AND CDN.TwrZasobyMag.TZM_MagNumer = 41 
                    LEFT JOIN CDN.MagAdresy ON CDN.TwrZasobyMag.TZM_MgAId = CDN.MagAdresy.MgA_Id  
                    WHERE Twr_kod = ''{_twrkod}'' 
                    order by tzm_ilosc desc'";
            //and isnull(MgA_Id,0)<>3171
            var movies = await App.TodoManager.PobierzDaneZWeb<TwrKarty>(Webquery);

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
