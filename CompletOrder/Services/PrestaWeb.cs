using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;
using CompletOrder.Models;
using CompletOrder.Models.QuickType;
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
    public class PrestaWeb
    {

        string BaseUrl = "https://www.szachownica.com.pl/api/";
        string Account = "G9E9HM7AEP2UYLZYK6XATBA7HENMPX31";
        string Password = "";
        OrderFactory orderFactory;
        CustomerFactory customer;
        OrderStateFactory orderStateFactory;

        public PrestaWeb()
        {

            orderFactory = new OrderFactory(BaseUrl, Account, Password);

            customer = new CustomerFactory(BaseUrl, Account, Password);



            //DateTime StartDate = new DateTime(2020, 8, 1);
            //DateTime EndDate = new DateTime(2020, 8, 31);
            //Dictionary<string, string> filter = new Dictionary<string, string>();
            //string dFrom = string.Format("{0:yyyy-MM-dd HH:mm:ss}", StartDate);
            //string dTo = string.Format("{0:yyyy-MM-dd HH:mm:ss}", EndDate);
            ////filter.Add("date_add", "[" + dFrom + "," + dTo + "]");
            //filter.Add("current_state", "3;15");
            //List<long> PrestaSharpOrderIds = orderFactory.GetIdsByFilter(filter, "id_DESC", null);




            

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
            filterdata.Add("date_upd", "[" + dFrom2 + "," + dTo2 + "]");

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
                    if(new int[] { 15, 2, 3, 11 }.Contains((int)s.id))
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

                        //var _url = $"https://www.szachownica.com.pl/api/customers/{308}/";
                        //var uri = new Uri(_url);
                        //var odp = await wyślijGet(uri, pobierzParametryAutoryzacji(Account));

                        ////var jsno = deserializujJson < PrestaKlient.Root > (odp.Strumień);

                        //using (var reader = new System.IO.StreamReader(odp.Strumień))
                        //{
                        //    string responseText = reader.ReadToEnd();

                        //}

                        var cstm = customer1.Where(a => a.id == i.id_customer).Select(s => new customer { lastname = s.lastname, firstname = s.firstname }).FirstOrDefault() as customer;

                        var kolor = statuses.Where(a => a.id == i.current_state).Select(s => new Status { color = s.color, status=s.status }).FirstOrDefault() as Status;
                        prestas.Add(new Presta
                        {
                            ZaN_GIDNumer = (int)i.id,
                            ZaN_DataWystawienia = i.date_add,
                            ZaN_DokumentObcy = i.reference,
                            ZaN_FormaNazwa = i.payment,
                           // ZaN_SpDostawy = state.name[0].Value,
                            ZaN_SpDostawy = kolor.status,
                            WartoscZam = i.total_paid,
                            //KnA_Akronim = klient.firstname + ' ' + klient.lastname,
                            KnA_Akronim = cstm !=null? cstm.firstname.ToUpper().Substring(0,1) + ". " + cstm.lastname:"",
                            //Color = state.color,
                            Color =kolor.color,


                        });
                    }
                }

            return prestas;
            });


        }

        class Status
        {
            public int id { get; set; }
            public string status  { get; set; }
            public string color  { get; set; }

        }

        public async Task<ObservableCollection<Presta>> PobierzelementyZamówienia( int id )
        {
            //string _url = $"https://www.szachownica.com.pl/api/orders/1/";//?fulfillment.status=PROCESSING  //status=READY_FOR_PROCESSING
            //var uri = new Uri(_url);
            //var odp = await wyślijGet(uri, pobierzParametryAutoryzacji(Account) );
            return await Task.Run(() =>
            {
                ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();

                
                
                var zamowianie = orderFactory.Get((long)id);


                foreach (var i in zamowianie.associations.order_rows)
                {

                    var nazwa = i.product_name.Substring(0, i.product_name.IndexOf("- Kolor") - 1);
                    var rozmiar = i.product_name.Substring(i.product_name.IndexOf("- Rozmiar") + 2, i.product_name.Length - i.product_name.IndexOf("- Rozmiar") - 2);
                    var kolor =
                        i.product_name.Substring(i.product_name.IndexOf("- Kolor") + 2, i.product_name.Length - i.product_name.IndexOf("- Kolor") - 2).Replace("- " + rozmiar, "");



                    prestas.Add(new Presta
                    {
                        ZaN_GIDNumer = (int)i.id,
                        ZaE_Ilosc = i.product_quantity,
                        ZaE_TwrKod = i.product_reference,
                        ZaE_TwrNazwa = i.product_name,
                        ElementId = (int)i.product_id,
                        WartoscZam=i.unit_price_tax_incl,
                        Kolor= kolor,
                        Rozmiar=rozmiar

                    });
                }

                return prestas;
            });


        }





        private async Task<Odpowiedź> wyślijGet(Uri url, string nagłówekAutoryzacji)
        {
            
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient klient = new HttpClient();
            klient.BaseAddress = url;
            klient.DefaultRequestHeaders.Clear();
            klient.DefaultRequestHeaders.Add("Authorization", nagłówekAutoryzacji);
            //klient.DefaultRequestHeaders.Authorization = cc;
            klient.DefaultRequestHeaders.Add("Output-Format", "JSON"); 

            HttpResponseMessage odp = await get(klient, url);
            var odpowiedź = Odpowiedź.Inicjacja(odp.StatusCode, odp.IsSuccessStatusCode, odp.Content.ReadAsStreamAsync().Result, null);
            return odpowiedź;
        }

        private async Task<HttpResponseMessage> get(HttpClient klient, Uri url)
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
