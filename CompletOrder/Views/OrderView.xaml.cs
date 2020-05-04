using CompletOrder.Models;
using CompletOrder.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompletOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderView : ContentPage
    {
         
        OrderViewModel orderView;
        private bool _userTapped;

        public OrderView()
        {
            InitializeComponent();

            BindingContext = orderView = new OrderViewModel();

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


            
           // var odp =await RodzajeMetod.WejdżWZamowienie(orderVM.id, orderVM.data);
            if (true)
                await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM)));
            else
                await DisplayAlert("info", "To zamówienie jest edytowane", "OK");


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            _userTapped = false;
        }



        protected override void OnAppearing()
        {
            orderView.PobierzListe(); 
            base.OnAppearing();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                orderView.OrderList.Where(x => x.status == "zapłacone");
               
            }
            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
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
