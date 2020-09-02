﻿using CompletOrder.Models;
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
    public partial class OrderView : TabbedPage
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


            
            var odp =await RodzajeMetod.WejdżWZamowienie(orderVM.id, orderVM.data);
            if (odp)
                await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM)));
            else 
            { 
                var odp2=await DisplayAlert("info", "To zamówienie jest edytowane\n Czy nadal chcesz je otworzyć?", "Tak","Nie");
                if(odp2)
                    await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM)));
            }


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            _userTapped = false;
        }



        protected override void OnAppearing()
        {
            orderView.PobierzListeZatwierdzonychZamowien();

            // orderView.PobierzListe();




            if(orderView.PrestaNagList.Count==0)
                orderView.GetPrestaZam();
           // else
             //   orderView.GetPrestaZam(true);
            orderView.PobierzAllegro(); 

            base.OnAppearing();
        }

         

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
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
                await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM)));
            else
            {
                var odp2 = await DisplayAlert("info", "To zamówienie jest edytowane\n Czy nadal chcesz je otworzyć?", "Tak", "Nie");
                if (odp2)
                    await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM)));
            }


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            _userTapped = false;
        }

        private async void MyListView3_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_userTapped)
                return;

            _userTapped = true;

            if (e.Item == null)
                return;
            var orderVM = e.Item as Presta;



            var odp = await RodzajeMetod.WejdżWZamowienie(orderVM.ZaN_GIDNumer, orderVM.ZaN_DataWystawienia);
            if (odp)
                await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM)));
            else
            {
                var odp2 = await DisplayAlert("info", "To zamówienie jest edytowane\n Czy nadal chcesz je otworzyć?", "Tak", "Nie");
                if (odp2)
                    await Navigation.PushAsync(new OrderDetailView(new OrderDetailVM(orderVM)));
            }


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            _userTapped = false;
        }
            
        private void MyListView3_Refreshing(object sender, EventArgs e)
        {
            orderView.GetPrestaZam(false);
            orderView.PobierzListeZatwierdzonychZamowien();
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