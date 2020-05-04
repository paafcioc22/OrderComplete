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

            BindingContext = Application.Current;

            entry_haslo.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            wersja_label.Text = $"ver {AppInfo.VersionString}";
        }

        class Haslo
        {
            public static string pass = "j0@rt";
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            if (entry_haslo.Text == Haslo.pass)
            {
                Navigation.PushAsync(new OrderView());
            }
            else
            {
                DisplayAlert(null, "Błędne hasło", "OK");
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (entry_haslo.Text == Haslo.pass)
            {
                Navigation.PushAsync(new OrderView());
            }
            else
            {
                DisplayAlert(null, "Błędne hasło", "OK");
            }
        }
    }
}