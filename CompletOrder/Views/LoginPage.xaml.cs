using CompletOrder.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompletOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
       
        public LoginPage()
        {
            InitializeComponent();

            BindingContext =  Application.Current;

            entry_haslo.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            wersja_label.Text = $"ver {AppInfo.VersionString}";
        }

        class Haslo
        {
            public static string pass = "j0@rt";
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            if (entry_haslo.Text == Haslo.pass)
            {
                //PrestaWeb prestaWeb = new PrestaWeb();
                //await prestaWeb.PobierzZamówienia();
                
                this.IsBusy = true;
                await Navigation.PushAsync(new OrderView());
                this.IsBusy = false;

            }
            else
            {
                await DisplayAlert(null, "Błędne hasło", "OK");
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            
            
            if (entry_haslo.Text == Haslo.pass)
            {
                this.IsBusy = true;
                await  Navigation.PushAsync(new OrderView());
                this.IsBusy = false;
            }
            else
            {
                await  DisplayAlert(null, "Błędne hasło", "OK");
            }
        }
    }
}