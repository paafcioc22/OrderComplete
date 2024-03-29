﻿using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;
using CompletOrder.Models;
using CompletOrder.Models.QuickType;
using CompletOrder.Views;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CompletOrder.Services
{
    public class PrestaWeb : DataBaseConn
    {

        protected string BaseUrl = "https://www.szachownica.com.pl/api/";
        protected string Account = "G9E9HM7AEP2UYLZYK6XATBA7HENMPX31";
        protected string Password = "";
        OrderFactory orderFactory;
        CustomerFactory customer;
        OrderStateFactory orderStateFactory;

        public PrestaWeb()
        {

            //try
            //{
            //    orderFactory = new OrderFactory(BaseUrl, Account, Password);

            //    customer = new CustomerFactory(BaseUrl, Account, Password);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

        }


        private bool OpenConnection()
        {
            try
            {
                mysqlconn.OpenAsync();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.

                return false;
                throw ex;
            }
        }


        public async Task<ObservableCollection<Presta>> PobierzZamówieniaSql()
        {
            var filtry = Application.Current as App;
            DateTimeOffset dateOd = new DateTime(filtry.DataOd.Year, filtry.DataOd.Month, filtry.DataOd.Day, 0, 0, 0);
            DateTimeOffset dateDo = new DateTime(filtry.DataDo.Year, filtry.DataDo.Month, filtry.DataDo.Day, 23, 59, 59);
            var sort = filtry.SortASC ? "ASC" : "DESC";
            var dataod = dateOd.ToString("yyyy-MM-dd HH:mm:ss");
            var datado = dateDo.ToString("yyyy-MM-dd HH:mm:ss");

            string filtr = "";

            if (!string.IsNullOrEmpty(SettingsPage.SendMetod))
            {
                if (SettingsPage.SendMetod != "!Wszystkie")


                    filtr = $"and pc.name='{SettingsPage.SendMetod}'";
            }

            string typyplatnosci = "";

            switch (filtry.WybranyTypPlatnosci)
            {
                case 0:
                    typyplatnosci = "15,2,3,11";
                    break;
                case 1:
                    typyplatnosci = "2";
                    break;
                case 2:
                    typyplatnosci = "3";
                    break;
                case 3:
                    typyplatnosci = "11";
                    break;
                case 4:
                    typyplatnosci = "15";
                    break;
            }

            ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(conn_string.ToString()))
                {
                    connection.Open();


                    // this.mysqlconn.Open();

                    //var cdsa=this.mysqlconn.Ping();

                    MySqlCommand command1 = connection.CreateCommand();
                    command1.CommandText = $@"SELECT id_order,reference,payment,ps_orders.date_add,ps_order_state.color,
                        CONVERT(total_paid,decimal(10,2))Totalpay, firstname, lastname , pc.name typDostawy, ps_order_state_lang.name
                        from ps_orders 
                        join ps_customer on ps_customer.id_customer=ps_orders.id_customer 
                        join ps_carrier pc on pc.id_carrier=ps_orders.id_carrier
                        join ps_order_state on ps_order_state.id_order_state= ps_orders.current_state                
                        join `ps_order_state_lang` on ps_order_state_lang.id_order_state= ps_order_state.id_order_state
                        where ps_orders.current_state in({typyplatnosci}) and ps_order_state_lang.id_lang=1 and
                        ps_orders.date_add BETWEEN '{dataod}' and '{datado}' {filtr} 
                        order by id_order {sort}";
                    MySqlDataReader reader = command1.ExecuteReader();

                    return await Task.Run(() =>
                    {
                        while (reader.Read())
                        {

                            var cstm = reader["firstname"].ToString().ToUpper().Substring(0, 1) + ". " + reader["lastname"].ToString();
                            DateTime dateTime;
                            var datazam = DateTime.TryParse(reader["date_add"].ToString(), out dateTime);

                            prestas.Add(new Presta
                            {
                                ZaN_GIDNumer = reader.GetInt32("id_order"),
                                ZaN_DataWystawienia = dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                ZaN_DokumentObcy = reader["reference"].ToString(),
                                ZaN_FormaNazwa = reader["payment"].ToString(),
                                ZaN_StatusPlatnosc = reader["name"].ToString(),
                                ZaN_SpDostawy = reader["typDostawy"].ToString(),
                                WartoscZam = reader.GetDecimal("Totalpay"),
                                KnA_Akronim = cstm,
                                Color = reader["color"].ToString()

                            });
                        }
                        //   this.mysqlconn.Close();

                        //this.mysqlconn = null;
                        return prestas;
                    });
                }
            }
            catch (Exception s)
            {
                //this.mysqlconn.Close();
                //this.mysqlconn = null;
                return prestas;

            }
        }



        public async Task<ObservableCollection<Presta>> PrestaPobierzListeKurierow()
        {
            var filtry = Application.Current as App;
            DateTimeOffset dateOd = new DateTime(filtry.DataOd.Year, filtry.DataOd.Month, filtry.DataOd.Day, 0, 0, 0);
            DateTimeOffset dateDo = new DateTime(filtry.DataDo.Year, filtry.DataDo.Month, filtry.DataDo.Day, 23, 59, 59);
            var sort = filtry.SortASC ? "ASC" : "DESC";
            var dataod = dateOd.ToString("yyyy-MM-dd HH:mm:ss");
            var datado = dateDo.ToString("yyyy-MM-dd HH:mm:ss");

            string typyplatnosci = "";

            switch (filtry.WybranyTypPlatnosci)
            {
                case 0:
                    typyplatnosci = "15,2,3,11";
                    break;
                case 1:
                    typyplatnosci = "2";
                    break;
                case 2:
                    typyplatnosci = "3";
                    break;
                case 3:
                    typyplatnosci = "11";
                    break;
                case 4:
                    typyplatnosci = "15";
                    break;
            }


            ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();
            try
            {
                this.mysqlconn.Open();
                MySqlCommand command1 = this.mysqlconn.CreateCommand();
                command1.CommandText = $@"SELECT distinct  pc.name typDostawy 
                from ps_orders 
                join ps_customer on ps_customer.id_customer=ps_orders.id_customer 
                join ps_carrier pc on pc.id_carrier=ps_orders.id_carrier
                join ps_order_state on ps_order_state.id_order_state= ps_orders.current_state                
                join `ps_order_state_lang` on ps_order_state_lang.id_order_state= ps_order_state.id_order_state
                where ps_orders.current_state in({typyplatnosci}) and ps_order_state_lang.id_lang=1 and
                ps_orders.date_add BETWEEN '{dataod}' and '{datado}'
                 union all select '!Wszystkie' order by 1";
                MySqlDataReader reader = command1.ExecuteReader();

                return await Task.Run(() =>
                {
                    while (reader.Read())
                    {

                        prestas.Add(new Presta
                        {

                            ZaN_SpDostawy = reader["typDostawy"].ToString(),

                        });
                    }
                    reader.Close();
                    this.mysqlconn.Close();
                    return prestas;
                });
            }
            catch (Exception s)
            {

                return prestas;
            }
        }


        public async Task<ObservableCollection<Presta>> PobierzZamówienia()
        {
            var filtry = Application.Current as App;

            orderStateFactory = new OrderStateFactory(BaseUrl, Account, Password);
            order_state state = new order_state();
            //customer klient = new customer();

            Dictionary<string, string> filter = new Dictionary<string, string>();
            string dFrom = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"));
            string dTo = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss"));
            filter.Add("date_upd", "[" + dFrom + "," + dTo + "]");


            DateTimeOffset date2 = new DateTime(filtry.DataDo.Year, filtry.DataDo.Month, filtry.DataDo.Day, 23, 59, 59);
            Dictionary<string, string> filterdata = new Dictionary<string, string>();
            string dFrom2 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", filtry.DataOd.ToString("yyyy-MM-dd HH:mm:ss"));
            string dTo2 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", date2.ToString("yyyy-MM-dd HH:mm:ss"));
            filterdata.Add("date_add", "[" + dFrom2 + "," + dTo2 + "]");

            return await Task.Run(() =>
            {
                ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();
                //var customer1 = customer.GetByFilter(filter, "date_upd_DESC", null);
                var customer1 = customer.GetByFilter(filter, "date_upd_DESC", null);

                //if(odp.RezultatOk)
                //{
                //    var objJson2 = deserializujJson<PrestaOrder>(odp.Strumień);
                //    _url = $"https://www.szachownica.com.pl/api/customers/{objJson2.Order.IdCustomer}/";//?fulfillment.status=PROCESSING  //status=READY_FOR_PROCESSING
                //    odp = await wyślijGet(uri, pobierzParametryAutoryzacji(Account));

                //    using (var reader = new System.IO.StreamReader(odp.Strumień))
                //    {
                //        string responseText = reader.ReadToEnd();

                //    }
                //}
                var app = Application.Current as App;
                var sort = app.SortASC ? "id_ASC" : "id_DESC";

                var zamowianie = orderFactory.GetByFilter(filterdata, sort, null);
                var state2 = orderStateFactory.GetAll();
                List<Status> statuses = new List<Status>();

                foreach (var s in state2)
                {
                    if (new int[] { 15, 2, 3, 11 }.Contains((int)s.id))
                    {
                        statuses.Add(new Status
                        {
                            id = (int)s.id,
                            color = s.color,
                            status = s.name[0].Value
                        });
                    }
                }

                foreach (var i in zamowianie)
                {

                    if (new int[] { 15, 2, 3, 11 }.Contains((int)i.current_state))
                    {
                        // i.date_upd
                        //state = orderStateFactory.Get((long)i.current_state);
                        //klient =  customer.GetAsync((long)i.id_customer).Result;


                        //var customer1 = customer.GetByFilter(filter, null, null);

                        var _url = $"https://www.szachownica.com.pl/api/customers/{308}/";
                        var uri = new Uri(_url);
                        //var odp = await wyślijGet(uri, pobierzParametryAutoryzacji(Account));

                        ////var jsno = deserializujJson < PrestaKlient.Root > (odp.Strumień);

                        //using (var reader = new System.IO.StreamReader(odp.Strumień))
                        //{
                        //    string responseText = reader.ReadToEnd();

                        //}

                        customer cstm = customer1.Where(a => a.id == i.id_customer).Select(s => new customer { lastname = s.lastname, firstname = s.firstname }).FirstOrDefault() as customer;

                        var kolor = statuses.Where(a => a.id == i.current_state).Select(s => new Status { color = s.color, status = s.status }).FirstOrDefault() as Status;
                        prestas.Add(new Presta
                        {
                            ZaN_GIDNumer = (int)i.id,
                            //  ZaN_DataWystawienia = i.date_add,
                            ZaN_DokumentObcy = i.reference,
                            ZaN_FormaNazwa = i.payment,
                            // ZaN_SpDostawy = state.name[0].Value,
                            ZaN_SpDostawy = kolor.status,
                            WartoscZam = i.total_paid,
                            //KnA_Akronim = klient.firstname + ' ' + klient.lastname,
                            KnA_Akronim = cstm != null ? cstm.firstname.ToUpper().Substring(0, 1) + ". " + cstm.lastname : "",
                            //Color = state.color,
                            Color = kolor.color,


                        });
                    }
                }

                return prestas;
            });


        }

        class Status
        {
            public int id { get; set; }
            public string status { get; set; }
            public string color { get; set; }

        }

        #region old

        public async Task<ObservableCollection<Presta>> PobierzelementyZamówienia(int id)
        {
            //string _url = $"https://www.szachownica.com.pl/api/orders/1/";//?fulfillment.status=PROCESSING  //status=READY_FOR_PROCESSING
            //var uri = new Uri(_url);
            //var odp = await wyślijGet(uri, pobierzParametryAutoryzacji(Account) );

            try
            {
                return await Task.Run(() =>
                    {
                        ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();



                        var zamowianie = orderFactory.Get((long)id);


                        foreach (var i in zamowianie.associations.order_rows)
                        {
                            var nazwa = "";
                            var rozmiar = " ";
                            var kolor = "";

                            //var dsadas = i.product_name;
                            //var sdasdsa = i.product_name.IndexOf("- Kolor");
                            //var sdrzomsda = i.product_name.IndexOf("- Rozmiar");


                            if (i.product_name.IndexOf("- Rozmiar") > 0)
                            {
                                rozmiar = i.product_name.Substring(i.product_name.IndexOf("- Rozmiar") + 2, i.product_name.Length - i.product_name.IndexOf("- Rozmiar") - 2);
                            }
                            if (i.product_name.IndexOf("- Kolor") > 0)
                            {
                                nazwa = i.product_name.Substring(0, i.product_name.IndexOf("- Kolor") - 1);

                                kolor =
                                    i.product_name.Substring(i.product_name.IndexOf("- Kolor") + 2, i.product_name.Length - i.product_name.IndexOf("- Kolor") - 2);


                                var podmien = "- " + rozmiar != " " ? "- " + rozmiar : " ";

                                kolor = kolor.Replace(podmien, " ");
                            }


                            prestas.Add(new Presta
                            {
                                ZaN_GIDNumer = id,
                                ZaE_Ilosc = i.product_quantity,
                                ZaE_TwrKod = i.product_reference,
                                ZaE_TwrNazwa = i.product_name,
                                ElementId = (int)i.id,
                                WartoscZam = i.unit_price_tax_incl,
                                Kolor = string.IsNullOrEmpty(kolor) ? "" : kolor,
                                Rozmiar = string.IsNullOrEmpty(rozmiar) ? "" : rozmiar

                            });
                        }

                        return prestas;
                    });
            }
            catch (Exception)
            {

                throw;
            }


        }


        #endregion

        public async Task<ObservableCollection<Presta>> MySqlPobierzelementyZamówienia(int id)
        {


            try
            {
                return await Task.Run(() =>
                {

                    this.mysqlconn.Open();
                    MySqlCommand command1 = this.mysqlconn.CreateCommand();
                    command1.CommandText = $@"SELECT i.id_order
                        ,id_order_detail
                        ,i.product_quantity
                        ,i.product_reference
                        ,i.product_name
                        ,i.unit_price_tax_incl
                        FROM ps_order_detail i
                        JOIN ps_orders o on o.id_order=i.id_order
                        where o.id_order={id}";


                    MySqlDataReader reader = command1.ExecuteReader();


                    ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();
                    while (reader.Read())
                    {
                        var nazwa = "";
                        var rozmiar = " ";
                        var kolor = "";



                        var product_name = reader["product_name"].ToString();
                        var twr_kod = reader["product_reference"]
                            .ToString()
                            .Replace("202CCTPULL-WF/SS/23-CH", "202CCTPULL-WF/SS/23")
                            .Replace("202CCTPULL-WF/SS/23-D", "202CCTPULL-WF/SS/23");  

                        if (product_name.IndexOf("- Rozmiar") > 0)
                        {
                            rozmiar = product_name.Substring(product_name.IndexOf("- Rozmiar") + 2, product_name.Length - product_name.IndexOf("- Rozmiar") - 2);
                        }
                        if (product_name.IndexOf("- Kolor") > 0)
                        {
                            nazwa = product_name.Substring(0, product_name.IndexOf("- Kolor") - 1);

                            kolor =
                                product_name.Substring(product_name.IndexOf("- Kolor") + 2, product_name.Length - product_name.IndexOf("- Kolor") - 2);


                            var podmien = "- " + rozmiar != " " ? "- " + rozmiar : " ";

                            kolor = kolor.Replace(podmien, " ");
                        }

                        prestas.Add(new Presta
                        {
                            ZaN_GIDNumer = Convert.ToInt32(reader["id_order"]),
                            ZaE_Ilosc = Convert.ToInt32(reader["product_quantity"]),
                            ZaE_TwrKod = twr_kod,
                            ZaE_TwrNazwa = reader["product_name"].ToString(),
                            ElementId = Convert.ToInt32(reader["id_order_detail"]),
                            WartoscZam = Convert.ToDecimal(reader["unit_price_tax_incl"]),
                            Kolor = string.IsNullOrEmpty(kolor) ? "" : kolor,
                            Rozmiar = string.IsNullOrEmpty(rozmiar) ? "" : rozmiar

                        });
                    }
                    this.mysqlconn.Close();
                    return prestas;


                });
            }
            catch (Exception)
            {

                throw;
            }


        }





        //private async Task<Odpowiedź> wyślijGet(Uri url, string nagłówekAutoryzacji)
        //{

        //    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    HttpClient klient = new HttpClient
        //    {
        //        BaseAddress = url
        //    };
        //    klient.DefaultRequestHeaders.Clear();
        //    klient.DefaultRequestHeaders.Add("Authorization", nagłówekAutoryzacji);
        //    //klient.DefaultRequestHeaders.Authorization = cc;
        //    klient.DefaultRequestHeaders.Add("Output-Format", "JSON"); 

        //    HttpResponseMessage odp = await Get(klient, url);
        //    var odpowiedź = Odpowiedź.Inicjacja(odp.StatusCode, odp.IsSuccessStatusCode, odp.Content.ReadAsStreamAsync().Result, null);
        //    return odpowiedź;
        //}

        private async Task<HttpResponseMessage> Get(HttpClient klient, Uri url)
        {

            return await klient.GetAsync(url);
        }

        private string pobierzParametryAutoryzacji(string idKlienta)
        {
            string idks = idKlienta + ":";
            byte[] bajty = Encoding.UTF8.GetBytes(idks);
            return "Basic " + Convert.ToBase64String(bajty);
        }


        private T deserializujJson<T>(Stream strumień)
        {
            var set = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss.SSSZ"),
            };
            var serializator = new DataContractJsonSerializer(typeof(T), set);
            T obiekt = (T)serializator.ReadObject(strumień);
            strumień.Flush();
            strumień.Close();
            return obiekt;
        }

    }

    public class Odpowiedź
    {
        public static Odpowiedź Inicjacja(HttpStatusCode status, bool rezultat, Stream strumień, string path) => new Odpowiedź(status, rezultat, strumień, path);

        public Stream Strumień { get; private set; }
        public HttpStatusCode Status { get; private set; }
        public bool RezultatOk { get; private set; }
        public string Path { get; private set; }

        private Odpowiedź(HttpStatusCode status, bool rezultat, Stream strumień, string path)
        {
            Status = status;
            Strumień = strumień;
            RezultatOk = rezultat;
            Path = path;
        }
    }


}
