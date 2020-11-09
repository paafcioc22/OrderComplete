using CompletOrder.Models;
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
        public SettingsPage()
        {
            InitializeComponent();

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

            PickerSource.ItemsSource = listaPlatnosci;

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
    }
}