using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CompletOrder.Models;
using Java.Util;

namespace CompletOrder.Droid
{


    public sealed class WebSerwisSzacho : IWebService
    {


        public WebSzacho.CDNOffLineSrv client;
        public List<TwrKarty> TowarInfoList { get; private set; }
        public ObservableCollection<Allegro> AllegroList { get; private set; }
        List<SendOrder> TowarInfo;
        public ObservableCollection<Presta> PrestaList { get; private set; }

        string odp;

        public WebSerwisSzacho()
        {
            client = new WebSzacho.CDNOffLineSrv();
        }

        public async Task<List<TwrKarty>> GetInfos(string query)
        {
            return await Task.Run(() =>
              {
                  TowarInfoList = new List<TwrKarty>();

                  var respone = client.ExecuteSQLCommand(query);

                  XmlDocument xmlDoc = new XmlDocument();
                  xmlDoc.LoadXml(respone);

                  TextReader reader = new StringReader(respone);

                  XmlSerializer serializer = new XmlSerializer(typeof(GidNazwa));
                  GidNazwa gidNazwa = (GidNazwa)serializer.Deserialize(reader);

                  foreach (var akcje in gidNazwa.TwrInfoLista)
                  {
                      //var datastart = Convert.ToDateTime(akcje.PzData).AddHours(2);

                      //int _MgA_Segment1;
                      //int _MgA_Segment2;
                      //int _MgA_Segment3;


                      //bool tak1 = Int32.TryParse(akcje.MgA_Segment1.ToString(), out  _MgA_Segment1);
                      //bool tak2 = Int32.TryParse(akcje.MgA_Segment2.ToString(), out _MgA_Segment2);
                      //bool tak3 = Int32.TryParse(akcje.MgA_Segment3.ToString(), out _MgA_Segment3);

                      TowarInfoList.Add(new TwrKarty
                      {
                          TwrKod = akcje.TwrKod,
                          Polozenie = akcje.Polozenie,
                          TwrUrl = akcje.TwrUrl,
                          //MgA_Segment1 = tak1 ? _MgA_Segment1 : 0,
                          //MgA_Segment2 = tak2 ? _MgA_Segment2 : 0,
                          //MgA_Segment3 = tak3 ? _MgA_Segment3 : 0
                          MgA_Segment1 = akcje.MgA_Segment1,
                          MgA_Segment2 = akcje.MgA_Segment2,
                          MgA_Segment3 = akcje.MgA_Segment3,
                          TwrStan=akcje.TwrStan

                      });
                  }

                  return TowarInfoList;
              });

        }


        public async Task<string> InsertOrderSend(SendOrder sendOrder)
        {
            try
            {
                return await Task.Run(() =>
                {
                     
                        var InsertString = $@"cdn.PC_InsertOrderNag
                                {sendOrder.Orn_OrderId},
                                '{sendOrder.Orn_IsDone}',
                                '{sendOrder.Orn_IsEdit}',
                                {sendOrder.Orn_DoneUser},
                                {sendOrder.Orn_EditUser},
                                '{sendOrder.Orn_OrderData}',
                                '{sendOrder.Orn_DoneData}',
                                '{sendOrder.Orn_DeviceId}'";

                        var respone = client.ExecuteSQLCommand(InsertString);

                        odp = respone;
                        odp = odp.Replace("<ROOT>\r\n  <Table>\r\n    <statuss>", "").Replace("</statuss>\r\n  </Table>\r\n</ROOT>", "");
                     
                    return odp;
                });
            }
            catch (Exception s)
            {

                odp = s.Message;
                return odp;
            }
        }

        

        public async Task<List<SendOrder>> SelectOrderSend(string query3)
        {
            
            TowarInfo = new List<SendOrder>();

            return await Task.Run(() =>
            {

                var respone = client.ExecuteSQLCommand(query3);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(respone);

                TextReader reader = new StringReader(respone);

                
                XmlSerializer serializer = new XmlSerializer(typeof(GetOrders));
                //
                GetOrders gidNazwa = (GetOrders)serializer.Deserialize(reader);

                //XmlSerializer serializer = new XmlSerializer(typeof(List<SendOrder>), new XmlRootAttribute("ROOT"));
                //List<SendOrder> gidNazwa = (List<SendOrder>)serializer.Deserialize(reader);


                foreach (var akcje in gidNazwa.OrderLista)
                {
                    TowarInfo.Add
                    ( 
                        new SendOrder
                        {
                            Orn_OrderId = akcje.Orn_OrderId,
                            Orn_IsDone = akcje.Orn_IsDone,
                            Orn_IsEdit = akcje.Orn_IsEdit,
                            Orn_DoneUser= akcje.Orn_DoneUser,
                            Orn_EditUser = akcje.Orn_EditUser
                        }  
                    );
                     
                } 

                return TowarInfo;
            });
        }

        public async Task<ObservableCollection<Allegro>> GetAllegros(string query3)
        {
            try
            {
                return await Task.Run(() =>
                    {
                        AllegroList = new ObservableCollection<Allegro>();

                        var respone = client.ExecuteSQLCommand(query3);

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(respone);

                        TextReader reader = new StringReader(respone);


                        XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Allegro>), new XmlRootAttribute("ROOT"));
                        ObservableCollection<Allegro> gidNazwa = (ObservableCollection<Allegro>)serializer.Deserialize(reader);

                        foreach (var a in gidNazwa)
                        {
                            var datastart = Convert.ToDateTime(a.RaportDate);
                            AllegroList.Add(new Allegro
                            {
                                Id = a.Id,
                                CustomerName = a.CustomerName,
                                kod = a.kod,
                                forma_platnosc = a.forma_platnosc,
                                nazwa = a.nazwa,
                                Pol1 = a.Pol1,
                                Pol2 = a.Pol2,
                                Pol3 = a.Pol3,
                                ilosc = a.ilosc,
                                RaportDate = datastart.ToString("yyyy-MM-dd"),
                                NrParagonu = a.NrParagonu,
                                ElementId = a.ElementId,
                                IsFinish = false
                            });
                        }

                        return AllegroList;
                    });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ObservableCollection<Presta>> GetPrestaZam(string query3)
        {
            try
            {
                return await Task.Run(() =>
                {
                    PrestaList = new ObservableCollection<Presta>();

                    var respone = client.ExecuteSQLCommand(query3);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(respone);

                    TextReader reader = new StringReader(respone);


                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Presta>), new XmlRootAttribute("ROOT"));
                    ObservableCollection<Presta> gidNazwa = (ObservableCollection<Presta>)serializer.Deserialize(reader);

                    foreach (var a in gidNazwa)
                    {
                        var datastart = Convert.ToDateTime(a.ZaN_DataWystawienia);
                        PrestaList.Add( a);
                    }

                    return PrestaList;
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }



    [XmlRoot("ROOT")]
    public class GidNazwa
    {
        [XmlElement("Table", typeof(TwrKarty))]
        public List<TwrKarty> TwrInfoLista { get; set; }
        //[XmlElement("Table", typeof(Allegro))]
        //public ObservableCollection<Allegro> AllegroLista { get; set; }
    }

  

    [XmlRoot("ROOT")]
    public class GetOrders
    {
        [XmlElement("Table", typeof(SendOrder))]
        public List<SendOrder> OrderLista { get; set; }
    }


}