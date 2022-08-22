using CompletOrder.Models;
using CompletOrder.Services;
using CompletOrder.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompletOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        static public List<TypPlatnosc> _typyPlatnosci;
        private List<string> listaPlatnosci;
        public static string SendMetod;
        string sklep;
        public OrderViewModel orderView;

        public SettingsPage(string sklep, OrderViewModel orderView)
        {
            InitializeComponent();
            this.orderView = orderView;

            if (sklep == "presta")
                label_metodywysylki.Title = label_metodywysylki.Title.Replace("Allegro", "Presta");
            _typyPlatnosci = new List<TypPlatnosc>();
            listaPlatnosci = new List<string>();

            _typyPlatnosci.Clear();
            _typyPlatnosci.Add(new TypPlatnosc { Id = 0, Typ_platnosc = "Wszystkie" });
            _typyPlatnosci.Add(new TypPlatnosc { Id = 1, Typ_platnosc = "Płatność zaakceptowana" });
            _typyPlatnosci.Add(new TypPlatnosc { Id = 2, Typ_platnosc = "Przygotowanie w toku" });
            _typyPlatnosci.Add(new TypPlatnosc { Id = 3, Typ_platnosc = "Płatność przyjęta" });
            _typyPlatnosci.Add(new TypPlatnosc { Id = 4, Typ_platnosc = "Płatność Przelewy24 przyjęta" });

            foreach (var item in _typyPlatnosci)
            {
                PickerSource.Items.Add(item.Typ_platnosc);
                listaPlatnosci.Add(item.Typ_platnosc);
            }

            this.sklep = sklep;
            PickerSource.ItemsSource = listaPlatnosci;
            var lista = Task.Run(()=> PobierzMetodyWsylki()).Result;
            PickerMetodaWysylki.ItemsSource = lista;
            PickerMetodaWysylki.SelectedItem = SendMetod;
            //PickerSource.SetBinding(Picker.ItemsSourceProperty, "TypPlatnosc"); 
            //PickerSource.ItemDisplayBinding = new Binding("Typ_platnosc");


            BindingContext = Application.Current;

              
        }

        private void PickerSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            var app = Application.Current as App;

            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                //var cos = (string)picker.ItemsSource[selectedIndex];
                app.WybranyTypPlatnosci = (sbyte)selectedIndex;
                 
            } 

        }

        async Task<List<string>> PobierzMetodyWsylki()
        {

            List<string> metody = new List<string>();

            try
            {
                if (sklep == "allegro")
                {
                    string tmp = $@"cdn.PC_WykonajSelect N' select distinct   typ_wysylka    from cdn.pc_allegroorders  union all select ''!Wszystkie'' order by 1'";

                    var wynikii = await App.TodoManager.GetOrdersFromAllegro(tmp);

                    foreach (var i in wynikii)
                    {
                        metody.Add(i.typ_wysylka);
                    }
                }
                else
                {
                    PrestaWeb prestaWeb = new PrestaWeb();
                    var  kurierzy = Task.Run(() => prestaWeb.PrestaPobierzListeKurierow()).Result;
                    foreach (var i in kurierzy)
                    {
                        metody.Add(i.ZaN_SpDostawy);
                    }
                }
               

               

            }
            catch (Exception)
            {
                 throw;
            }
            return metody;
        }


        bool maybe_exit = false;
        protected override bool OnBackButtonPressed()
        {
            //bool back = false;

            //if (!maybe_exit)
            //{
            //    maybe_exit = true;
            //    back = base.OnBackButtonPressed();
            //}
            //maybe_exit = false;
            //return back;


            if (!maybe_exit)
            {
              
                orderView.PobierzListeZatwierdzonychZamowien();

                orderView.GetPrestaZam();

                orderView.PobierzAllegro();

                maybe_exit = true;
                return false;
            }
            return true;



            //if (maybe_exit) 
            //    return false; //QUIT

            ////DependencyService.Get<Model.IAppVersionProvider>().ShowLong("Wciśnij jeszcze raz by wyjść z aplikacji");
            //maybe_exit = true;

            //Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            //{
            //    maybe_exit = false; //reset those 2 seconds
            //                        // false - Don't repeat the timer 
            //    return false;
            //});
            //return true;
        }

        private  void PickerMetodaWysylki_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if(PickerMetodaWysylki.SelectedItem!=null)
            SendMetod = PickerMetodaWysylki.SelectedItem.ToString();
        }

     
    }
}