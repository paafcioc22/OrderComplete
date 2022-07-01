using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CompletOrder.Services;
using CompletOrder.Views;
using System.Globalization;
using CompletOrder.ViewModels;
using System.Collections.Generic;
using CompletOrder.Models;
using Microsoft.AspNetCore.Identity;

namespace CompletOrder
{
    public partial class App : Application
    {

        public static WebMenager TodoManager { get; set; }
        

       // public TypPlatnosc typPlatnosci { get; set; } = new TypPlatnosc();
        //public OrderViewModel orderViewModel { get; set; } = new OrderViewModel();

        private const string filtrZaplacone = "filtrZaplacone";
        private const string filtrNowe = "filtrNowe";
        private const string filtrAnulowane = "filtrAnulowane";
        private const string filtrZrealizowane = "filtrZrealizowane";
        private const string filtrRealizowane = "filtrRealizowane";
        private const string filtrDataOd = "filtrDataOd";
        private const string filtrDataDo = "filtrDataDo";
        private const string ListaTypow = "ListaTypow";
        private const string baseName = "baseName";
        private const string isloading = "isloading";
        private const string passwordsql2 = "passwordsql2";

        public App()
        {
            InitializeComponent();
          
            DependencyService.Register<MockDataStore>();
             
            DependencyService.Register<IPasswordHasher<User>, Models.PasswordHasher<User>>();
            MainPage = new NavigationPage( new LoginPage());
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public string BaseName
        {
            get
            {
                if (Properties.ContainsKey(baseName))
                    return Properties[baseName].ToString();
                return "admin_hhdtret44";
            }
            set
            {
                Properties[baseName] = value;
            }

        }

        public string PasswordSQL
        {
            get
            {
                if (Properties.ContainsKey(passwordsql2))
                    return Properties[passwordsql2].ToString();
                return "x4Yfu$091";
            }
            set
            {
                Properties[passwordsql2] = value;
            }

        }


        public bool OnlyToDoKompletacja
        {
            get
            {
                if (Properties.ContainsKey(filtrZaplacone))
                    return (bool)Properties[filtrZaplacone];
                return false;
            }
            set
            {
                Properties[filtrZaplacone] = value;
              //  orderViewModel.Filtr = true;
            }

        }

        public bool FiltrNowe
        {
            get
            {
                if (Properties.ContainsKey(filtrNowe))
                    return (bool)Properties[filtrNowe];
                return false;
            }
            set
            {
                Properties[filtrNowe] = value;
            //    orderViewModel.Filtr = true;
            }

        }

        public bool FiltrAnulowane
        {
            get
            {
                if (Properties.ContainsKey(filtrAnulowane))
                    return (bool)Properties[filtrAnulowane];
                return false;
            }
            set
            {
                Properties[filtrAnulowane] = value;
              //  orderViewModel.Filtr = true;
            }

        }

        public bool SortASC
        {
            get
            {
                if (Properties.ContainsKey(filtrZrealizowane))
                    return (bool)Properties[filtrZrealizowane];
                return true;
            }
            set
            {
                Properties[filtrZrealizowane] = value;
             //   orderViewModel.Filtr = true;
            }

        }

        public bool FiltrRealizowane
        {
            get
            {
                if (Properties.ContainsKey(filtrRealizowane))
                    return (bool)Properties[filtrRealizowane];
                return false;
            }
            set
            {
                Properties[filtrRealizowane] = value;
             //   orderViewModel.Filtr = true;
            }

        }

        public DateTime DataOd
        {
            get
            {
                if (Properties.ContainsKey(filtrDataOd))
                    return (DateTime)Properties[filtrDataOd];
                return DateTime.Now.AddDays(-4);
            }
            set
            {
                Properties[filtrDataOd] = value;
             //   orderViewModel.Filtr = true;
            }

        }

        public DateTime DataDo
        {
            get
            {
                if (Properties.ContainsKey(filtrDataDo))
                    return (DateTime)Properties[filtrDataDo];
                return DateTime.Now;
            }
            set
            {
                Properties[filtrDataDo] = value;
             //   orderViewModel.Filtr = true;
            }

        }


        public sbyte WybranyTypPlatnosci
        {
            get
            {
                if (Properties.ContainsKey(ListaTypow))
                    return (sbyte)Properties[ListaTypow];
                return 0;
            }
            set
            {
                Properties[ListaTypow] = value;
            //    orderViewModel.Filtr = true;
            }

        }

        public bool IsLoading
        {
            get
            {
                if (Properties.ContainsKey(isloading))
                    return (bool)Properties[isloading];
                return true;
            }
            set
            {
                Properties[isloading] = value;
                //  orderViewModel.Filtr = true;
            }

        }



    }
}
