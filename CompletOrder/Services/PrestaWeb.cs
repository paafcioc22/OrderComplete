using Bukimedia.PrestaSharp.Entities;
using Bukimedia.PrestaSharp.Factories;
using CompletOrder.Models;
using CompletOrder.Models.QuickType;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

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
            //string _url = $"https://www.szachownica.com.pl/api/orders/1/";//?fulfillment.status=PROCESSING  //status=READY_FOR_PROCESSING
            //var uri = new Uri(_url);
            //var odp = await wyślijGet(uri, pobierzParametryAutoryzacji(Account) );
            return await Task.Run(() =>
            {
                ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();

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
                orderStateFactory = new OrderStateFactory(BaseUrl, Account, Password);
                var zamowianie = orderFactory.GetAll();


                foreach (var i in zamowianie)
                {
                    var state = orderStateFactory.Get((long)i.current_state);
                     
                    var klient = customer.Get((long)i.id_customer);
                    prestas.Add(new Presta
                    {
                        ZaN_GIDNumer = (int)i.id,
                        ZaN_DataWystawienia = i.date_add,
                        ZaN_DokumentObcy = i.reference,
                        ZaN_FormaNazwa = i.payment,
                        ZaN_SpDostawy = state.name[0].Value,
                        WartoscZam = i.total_paid,
                        KnA_Akronim = klient.firstname + ' ' + klient.lastname,
                        Color= state.color

                    });
                }

            return prestas;
            });


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
                     
                    prestas.Add(new Presta
                    {
                        ZaN_GIDNumer = (int)i.id,
                        ZaE_Ilosc = i.product_quantity,
                        ZaE_TwrKod = i.product_reference,
                        ZaE_TwrNazwa = i.product_name,
                        ElementId = (int)i.product_id,
                        WartoscZam=i.unit_price_tax_incl

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
