using CompletOrder.Models;
using CompletOrder.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompletOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderView : TabbedPage
    {
         
        public OrderViewModel orderView;
        private bool _userTapped;

        public OrderView()
        {
            InitializeComponent();
            Title= "User : " +Preferences.Get("user", "default_value");
            BindingContext = orderView = new OrderViewModel();

            orderView.PobierzListeZatwierdzonychZamowien();
          
            orderView.GetPrestaZam();
      
            orderView.PobierzAllegro();

            IDevice device = DependencyService.Get<IDevice>();
            string deviceIdentifier = device.GetIdentifier();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            IDevice device = DependencyService.Get<IDevice>();
            string deviceIdentifier = device.GetIdentifier();


            if (_userTapped)
                return;

            _userTapped = true;

            if (e.Item == null)
                return;
            var orderVM = e.Item as Order;


            
            var odp =await RodzajeMetod.WejdżWZamowienie(orderVM.id, orderVM.data);
            if (odp)
                await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM), orderView));
            else 
            { 
                var odp2=await DisplayAlert("info", "To zamówienie jest edytowane\n Czy nadal chcesz je otworzyć?", "Tak","Nie");
                if(odp2)
                    await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM), orderView));
            }


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            _userTapped = false;
        }


        


        //protected override void OnAppearing()
        //{
        //    var pages = Application.Current.MainPage.Navigation.ModalStack;

        //    if (pages.Count > 0)
        //    {
        //        var nazwa = pages[pages.Count - 1].GetType().Name;
        //    }


        //    orderView.PobierzListeZatwierdzonychZamowien();

        //    // orderView.PobierzListe(); 


        //   // if(orderView.PrestaNagList.Count==0)
        //    orderView.GetPrestaZam();
        //   // else
        //     //   orderView.GetPrestaZam(true);
        //    orderView.PobierzAllegro(); 

        //    base.OnAppearing();
        //}

         

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage("allegro", orderView));
        }

        private async void MyListView2_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_userTapped)
                return;

            _userTapped = true;

            if (e.Item == null)
                return;
            var orderVM = e.Item as Allegro;



            var odp = await RodzajeMetod.WejdżWZamowienie(orderVM.Id, orderVM.RaportDate);
            if (odp)
                await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM), orderView));
            else
            {
                var odp2 = await DisplayAlert("info", "To zamówienie jest edytowane\n Czy nadal chcesz je otworzyć?", "Tak", "Nie");
                if (odp2)
                    await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM), orderView));
            }


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            _userTapped = false;
        }

        private async void MyListView3_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                if (_userTapped)
                    return;

                _userTapped = true;

                if (e.Item == null)
                    return;
                var orderVM = e.Item as Presta;



                var odp = await RodzajeMetod.WejdżWZamowienie(orderVM.ZaN_GIDNumer, orderVM.ZaN_DataWystawienia.ToString());
                if (odp)
                    await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM), orderView));
                else
                {
                    var odp2 = await DisplayAlert("info", "To zamówienie jest edytowane\n Czy nadal chcesz je otworzyć?", "Tak", "Nie");
                    if (odp2)
                        await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM), orderView));
                }


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

                _userTapped = false;
            }
            catch (Exception)
            {

                throw;
            }
        }
            
        private void MyListView3_Refreshing(object sender, EventArgs e)
        {
            orderView.GetPrestaZam(false);
            orderView.PobierzListeZatwierdzonychZamowien();
        }

        private void MyListView2_Refreshing(object sender, EventArgs e)
        {
            orderView.PobierzAllegro();
            orderView.PobierzListeZatwierdzonychZamowien();
        }

        private async void Btn_prestaSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage("presta", orderView));
        }

        protected  override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert("Uwaga", "Czy chcesz się wylogować?", "Tak", "NIE");
                if (result) await this.Navigation.PopAsync(); // or anything else
            });

            return true;

            
        }

        //public IEnumerable<Order> SzukajTowar(string searchText = null)
        //{

        //    if (String.IsNullOrWhiteSpace(searchText))
        //        return orderView.OrderList;
        //    //return _listaOrder.Where(c => c.id.ToString().Contains(searchText));
        //    return orderView.OrderList.Where(c => c.id.ToString().Contains(searchText));

        //}

        //private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    orderView.OrderList = OrderDetailVM.Convert2(SzukajTowar(e.NewTextValue).ToList());  
        //}
    }
}
