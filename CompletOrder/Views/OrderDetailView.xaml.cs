using CompletOrder.Models;
using CompletOrder.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompletOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailView : ContentPage
    {

        OrderDetailVM orderDetail;
        private SQLiteAsyncConnection _connection;
        public string  DataOrder{ get; set; }
        static string deviceIdentifier;

        public OrderDetailView(OrderDetailVM orderDetailVM)
        {
            InitializeComponent();

            _connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();

            orderDetail = orderDetailVM;

            //((NavigationPage)Application.Current.MainPage).BarTextColor = Color.LightGray;

            DataOrder = orderDetailVM._order.data;

              BindingContext = orderDetail = orderDetailVM;
            _connection.CreateTableAsync<OrderComplete>();

            IDevice device = DependencyService.Get<IDevice>();
            deviceIdentifier = device.GetIdentifier();
            //var suma = orderDetail.orderDetail.Count();// (s => s.cena_netto);

            var ileall=orderDetail.PozycjiZamowienia;
            var ilePo = orderDetail.orderDetail.Count();
            var suma = Convert.ToDecimal(orderDetail.orderDetail.Sum(s => s.cena_netto*s.ilosc));

            if (ileall!=ilePo)
            //if (suma != orderDetail.SumaZamowienia)
                DisplayAlert("Uwaga", "Nie pełna lista zamówienia", "OK");
            lbl_sumaKwota.Text = $"Łączna kwota pozycji : {suma:N} zł";
            //CzyKtosNieRobi();
        }


        async void CzyKtosNieRobi()
        {

            string tmp = $@"cdn.PC_WykonajSelect N'select * from cdn.pc_ordernag where Orn_OrderId={orderDetail._orderid}'";

            var wynikii = await App.TodoManager.GetOrdersFromWeb(tmp);

            


            if (wynikii.Where(x=>x.Orn_IsEdit==true).Any())
                await DisplayAlert("Uwaga", "Ktoś już kompletuje to zamówienie", "OK");



        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            ////await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
            //var a = (sender as ListView).Parent.Parent as ViewCell;
            //var b = (sender as ListView).Parent as ViewCell;
            //var c = (sender as CheckBox).Parent.Parent as ViewCell;
            //var d = (sender as CheckBox).Parent as ViewCell;


            var order = e.Item as OrderDetail;

            //ViewCell cell = (sender as ListView).Parent.Parent as ViewCell; 
            //OrderDetail order = cell.BindingContext as OrderDetail;
            order.IsDone = !order.IsDone;


            ZaznaczElement(order, order.IsDone);

            var nowa = orderDetail.orderDetail.OrderBy(s => s.IsDone);

            MyListView.ItemsSource = nowa.ToList();

            if (orderDetail.orderDetail.Count() == orderDetail.orderDetail.Where(s => s.IsDone == true).Count())
            {
                this.Title += " >> ZAKOŃCZONE";

               
                var DataDone = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
                 
                var odp = await RodzajeMetod.ZakonczIwyjdz(orderDetail._orderid, DataDone);
                await Navigation.PopAsync();
            }
            else
            {
                this.Title = this.Title.Replace(" >> ZAKOŃCZONE", "");

               
               

                //var movies = await App.TodoManager.InsertOrderSend(send);

            }


            ////Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override async void OnAppearing()
        {

            await _connection.CreateTableAsync<OrderDatailComplete>();
            BindingContext = orderDetail;
            base.OnAppearing();
        }


        private async void ZaznaczElement(OrderDetail order, bool isClick)
        {

            var tmp2 = await _connection.QueryAsync<OrderDatailComplete>("select * from OrderDatailComplete where IdOrder = ?  and IdElementOrder=? ", order.OrderId, order.IdElement);
            var updejt = new OrderDatailComplete
            {
                
                IdOrder = order.OrderId,
                IdElementOrder = order.IdElement
            };


            if (isClick)
            {
                var zaznaczone = orderDetail.orderDetail.Where(x => x.OrderId == order.OrderId);

               
                var tmp = await _connection.QueryAsync<OrderDatailComplete>("select * from OrderDatailComplete where IdOrder = ?  and IdElementOrder=? ", order.OrderId, order.IdElement);
                if (tmp.Count == 0)
                    await _connection.InsertAsync(updejt);
            }
            else {

                
                var tmp = await _connection.QueryAsync<OrderDatailComplete>("select * from OrderDatailComplete where IdOrder = ?  and IdElementOrder=? ", order.OrderId, order.IdElement);
                var delete = new OrderDatailComplete
                {
                    Id=tmp[0].Id,
                    IdOrder = order.OrderId,
                    IdElementOrder = order.IdElement
                };
                await _connection.DeleteAsync(delete); //Uswuwasz znaczki zakonczenia
                var tmp3 = await _connection.QueryAsync<OrderDatailComplete>("select * from OrderDatailComplete where IdOrder = ?  and IdElementOrder=? ", order.OrderId, order.IdElement);

            }
        }


        private  void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //ViewCell cell = (sender as CheckBox).Parent.Parent as ViewCell;

            //OrderDetail order = cell.BindingContext as OrderDetail;



            //ZaznaczElement(order,e.Value); 
           

            //var nowa = orderDetail.orderDetail.OrderBy(s => s.IsDone);

            //MyListView.ItemsSource = nowa.ToList();

            //if (orderDetail.orderDetail.Count() == orderDetail.orderDetail.Where(s => s.IsDone == true).Count())
            //{
            //    this.Title += " >> ZAKOŃCZONE";

            //    //var _orderIdFinish = new OrderComplete { IdOrder = orderDetail._orderid };
            //    //var _orderIdFinish = new OrderComplete { IdOrder = orderDetail._orderid };
            //    var DataDone = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    var send = new SendOrder()
            //    {
            //        Orn_OrderId = orderDetail._orderid,
            //        Orn_IsDone = true,
            //        Orn_IsEdit = false,
            //        Orn_DoneUser = 0,
            //        Orn_EditUser = 0,
            //        Orn_OrderData = DataOrder,
            //        Orn_DoneData = DataDone,
            //        Orn_DeviceId= deviceIdentifier
            //    };

            //    var movies = await App.TodoManager.InsertOrderSend(send);
            //    var odp = await RodzajeMetod.ZakonczIwyjdz(orderDetail._orderid, DataDone);
            //    await Navigation.PopAsync();
            //}
            //else
            //{
            //    this.Title = this.Title.Replace(" >> ZAKOŃCZONE", "");

            //    //await _connection.DeleteAsync(_orderIdFinish); Uswuwasz znaczki zakonczenia
            //    var send = new SendOrder()
            //    {
            //        Orn_OrderId = orderDetail._orderid,
            //        Orn_IsDone = false,
            //        Orn_IsEdit = false,
            //        Orn_DoneUser = 0,
            //        Orn_EditUser = 0,
            //        Orn_OrderData = DataOrder,
            //        Orn_DoneData = null,

            //    };

            //    //var movies = await App.TodoManager.InsertOrderSend(send);

            //}
             

        }

        private async void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {

            var odp1 =await DisplayAlert("Pytanie..", "Czy chcesz zakończyć zadanie?", "Tak", "Nie");

            

            if (odp1)
            {
                
                var DataDone = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
               
                var odp = await RodzajeMetod.ZakonczIwyjdz(orderDetail._orderid, DataDone);
                await Navigation.PopAsync();

                //await _connection.InsertAsync(_orderIdFinish);
                
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            ViewCell cell = (sender as Label).Parent.Parent as ViewCell;

            OrderDetail order = cell.BindingContext as OrderDetail;

            

            List<string> nowy = new List<string>();
            var stwrkarty = Task.Run(() => orderDetail.GetTwrKartyAsync(order.kod));

            foreach (var wpis in stwrkarty.Result)
            {
                nowy.Add(String.Concat(wpis.Polozenie, " : ", (wpis.TwrStan), " szt"));
            }

            DisplayActionSheet("Wszystkie położenia:", "OK", null, nowy.ToArray());

            //DisplayAlert(null, "tu będą inne położenia", "ok");
        }



        async void TylkoWyjdz()
        {

            //var send = new SendOrder()
            //{
            //    Orn_OrderId = orderDetail._orderid,
            //    Orn_IsDone = false,
            //    Orn_IsEdit = false,
            //    Orn_DoneUser = 0,
            //    Orn_EditUser = 0,
            //    Orn_OrderData = DataOrder,


            //};

            //var movies = await App.TodoManager.InsertOrderSend(send);

            var odp = await RodzajeMetod.TylkoWyjdz(orderDetail._orderid);
        }

        protected override bool OnBackButtonPressed()
        {
            TylkoWyjdz();
            
            return base.OnBackButtonPressed();
        }

        private void TapGestureRecognizer_ShowKod(object sender, EventArgs e)
        {
            ViewCell cell = (sender as Label).Parent.Parent as ViewCell;

            OrderDetail order = cell.BindingContext as OrderDetail;

            DisplayAlert("Pełny kod towaru..", $"{order.kod}\n\n{order.nazwa}", "Ok");
        }
    }
}
