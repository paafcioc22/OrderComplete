using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompletOrder.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Bukimedia.PrestaSharp.Factories;
using System.Collections.ObjectModel;
using CompletOrder.Models;

namespace CompletOrder.Services.Tests
{
    [TestClass()]
    public class PrestaWebTests
    {
        private OrderStateFactory orderStateFactory;
        string BaseUrl = "https://www.szachownica.com.pl/api/";
        string Account = "G9E9HM7AEP2UYLZYK6XATBA7HENMPX31";
        string Password = "";
        OrderFactory orderFactory;
        CustomerFactory customer;


        [TestMethod()]
        public void PobierzZamówieniaTest()
        {
            orderStateFactory = new OrderStateFactory(BaseUrl, Account, Password);
             
                ObservableCollection<Presta> prestas = new ObservableCollection<Presta>();

               
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
                        Color = state.color,
                        

                    });
                }

                



            Assert.Fail();
        }
    }
}