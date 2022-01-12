using CompletOrder.Services;
using CompletOrder.ViewModels;
using Plugin.LatestVersion;
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

        LoginViewModel viewModel;
        public LoginPage()
        {
            InitializeComponent();

            //entry_haslo2.IsVisible = false;
            //BindingContext =  Application.Current;
            BindingContext = viewModel = new LoginViewModel();

            
            entry_haslo.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            wersja_label.Text = $"ver {AppInfo.VersionString}";


            SprNowaWersja();
        }


        class Haslo
        {
            public static string pass = "j0@rt";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadItemsCommand.Execute(null);
        }

        async void SprNowaWersja()
        {
            try
            {
                var isLatest = await CrossLatestVersion.Current.IsUsingLatestVersion();
                //string latestVersionNumber1 = await CrossLatestVersion.Current.GetLatestVersionNumber("com.SzachoToolsMini");
                //string latestVersionNumber = await CrossLatestVersion.Current.GetLatestVersionNumber();
                //string installedVersionNumber = CrossLatestVersion.Current.InstalledVersionNumber;

                if (!isLatest)
                {
                    var update = await DisplayAlert("Nowa wersja", "Dostępna nowa wersja. Chcesz pobrać?", "Tak", "Nie");

                    if (update)
                    {
                        await CrossLatestVersion.Current.OpenAppInStore();
                    }
                }
            }
            catch (Exception a)
            {

                await DisplayAlert("Uwaga", "Błąd sprawdzania wersji..Sprawdź połączenie", "OK");
            }
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            //if (entry_haslo.Text == Haslo.pass)
            //{
            //    //PrestaWeb prestaWeb = new PrestaWeb();
            //    //await prestaWeb.PobierzZamówienia();

            //    kolko.IsRunning = true;
            //    kolko.IsVisible = true;
            //    await Navigation.PushAsync(new OrderView());
            //    kolko.IsRunning = false;
            //    kolko.IsVisible = false;

            //}
            //else
            //{
            //    await DisplayAlert(null, "Błędne hasło", "OK");
            //}
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await btn_click.ScaleTo(1.1, 50, Easing.Linear);
            await Task.Delay(100);
            await btn_click.ScaleTo(1, 50, Easing.Linear);
            //await imageCircleBack.TranslateTo(0, -10, 100);
            //await imageCircleBack.TranslateTo(0, 0, 100);

            if (entry_haslo.Text == Haslo.pass)
            {
                kolko.IsRunning = true;
                kolko.IsVisible = true;
                 await  Navigation.PushAsync(new OrderView());
                kolko.IsRunning = false;
                kolko.IsVisible = false;
            }
            else
            {
                await  DisplayAlert(null, "Błędne hasło", "OK");
            }
        }

        private void PickerLogin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if(PickerLogin.SelectedIndex>=0)
              //  viewModel.LoadItemsCommand.Execute(null);

            entry_haslo.Text = "";
            entry_haslo2.Text = "";
        }
    }
}