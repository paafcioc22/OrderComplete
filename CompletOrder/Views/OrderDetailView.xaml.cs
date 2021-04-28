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

        OrderDetailVM orderDetailVm;
        //private SQLiteAsyncConnection _connection;
        public string  DataOrder{ get; set; }
        static string deviceIdentifier;

        public OrderDetailView(OrderDetailVM orderDetailVM)
        {
            InitializeComponent();

            //_connection = DependencyService.Get<SQLite.ISQLiteDb>().GetConnection();

            orderDetailVm = orderDetailVM;

             

            DataOrder = orderDetailVM._order.data;

              BindingContext = orderDetailVm = orderDetailVM;
            //_connection.CreateTableAsync<OrderComplete>();

            IDevice device = DependencyService.Get<IDevice>();
            deviceIdentifier = device.GetIdentifier();
          

            var ileall=orderDetailVm.PozycjiZamowienia;
            var ilePo = orderDetailVm.OrderDetail.Count();
            var suma = Convert.ToDecimal(orderDetailVm.OrderDetail.Sum(s => s.cena_netto*s.ilosc));

            if (ileall!=ilePo)
            //if (suma != orderDetail.SumaZamowienia)
                DisplayAlert("Uwaga", "Nie pełna lista zamówienia", "OK");
            lbl_sumaKwota.Text = $"Łączna kwota pozycji : {suma:N} zł";
            //CzyKtosNieRobi();
        }


        async void CzyKtosNieRobi()
        {

            string tmp = $@"cdn.PC_WykonajSelect N'select * from cdn.pc_ordernag where Orn_OrderId={orderDetailVm._orderid}'";

            var wynikii = await App.TodoManager.GetOrdersFromWeb(tmp);

            


            if (wynikii.Where(x=>x.Orn_IsEdit==true).Any())
                await DisplayAlert("Uwaga", "Ktoś już kompletuje to zamówienie", "OK");



        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

         
            var order = e.Item as OrderDetail;

    
            order.IsDone = !order.IsDone;


            ZaznaczElement(order, order.IsDone);

            var nowa = orderDetailVm.OrderDetail.OrderBy(s => s.IsDone);

            MyListView.ItemsSource = nowa.ToList();

            if (orderDetailVm.OrderDetail.Count() == orderDetailVm.OrderDetail.Where(s => s.IsDone == true).Count())
            {
                this.Title += " >> ZAKOŃCZONE";

               
                var DataDone = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
                 
                var odp = await RodzajeMetod.ZakonczIwyjdz(orderDetailVm._orderid, DataDone);
                await Navigation.PopAsync();
            }
            else
            {
                
                this.Title = this.Title.Replace(" >> ZAKOŃCZONE", "");  

            }


            ////Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override   void OnAppearing()
        {

           // await _connection.CreateTableAsync<OrderDatailComplete>();
            BindingContext = orderDetailVm;
            base.OnAppearing();
        }


        private async void ZaznaczElement(OrderDetail order, bool isClick)
        {
            PC_SiOrderElem orderElem;

            //var tmp2 = await _connection.QueryAsync<OrderDatailComplete>("select * from OrderDatailComplete where IdOrder = ?  and IdElementOrder=? ", order.OrderId, order.IdElement);

            var updejt = new OrderDatailComplete
            { 
                IdOrder = order.OrderId,
                IdElementOrder = order.IdElement
            };

            var tmp = await orderDetailVm.SelectOrderElem(order.OrderId, order.IdElement);

            if (isClick)
            {
                var zaznaczone = orderDetailVm.OrderDetail.Where(x => x.OrderId == order.OrderId);


                // var tmp = await _connection.QueryAsync<OrderDatailComplete>("select * from OrderDatailComplete where IdOrder = ?  and IdElementOrder=? ", order.OrderId, order.IdElement);

                if (tmp.Count == 0)
                {
                    //await _connection.InsertAsync(updejt);


                    orderElem = new PC_SiOrderElem()
                    {
                        OrE_PlaceName = order.twrkarty.Polozenie,
                        OrE_OrderEleId = order.IdElement,
                        OrE_OrderId = order.OrderId,
                        OrE_Quantity = order.ilosc,
                        OrE_MpaId = order.twrkarty.MgA_Id

                    };

                    await orderDetailVm.AddOrderElem(orderElem);
                }
            }
            else {

                
                //var tmp = await _connection.QueryAsync<OrderDatailComplete>("select * from OrderDatailComplete where IdOrder = ?  and IdElementOrder=? ", order.OrderId, order.IdElement);
                var delete = new OrderDatailComplete
                {
                    //Id=tmp[0].Id,
                    IdOrder = order.OrderId,
                    IdElementOrder = order.IdElement
                };
                //await _connection.DeleteAsync(delete); //Uswuwasz znaczki zakonczenia
                await orderDetailVm.DeleteOrderElem(delete);
           

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
               
                await RodzajeMetod.ZakonczIwyjdz(orderDetailVm._orderid, DataDone);
                await Navigation.PopAsync(); 
                
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            ViewCell cell = (sender as Label).Parent.Parent as ViewCell;

            OrderDetail order = cell.BindingContext as OrderDetail;

            

            List<string> nowy = new List<string>();

            var zam = orderDetailVm.OrderDetail.Where(s => s.kod == order.kod).FirstOrDefault();
            

            var stwrkarty = Task.Run(() => orderDetailVm.GetTwrKartyAsync(order.kod));

            foreach (var wpis in stwrkarty.Result)
            {
                nowy.Add(String.Concat(wpis.Polozenie, " : ", (wpis.TwrStan), " szt"));
            }

            var polozenie=await DisplayActionSheet("Wszystkie położenia:", "OK", "Anuluj", nowy.ToArray());

            if (polozenie != "Anuluj" && polozenie != "OK")
            {
                zam.twrkarty.Polozenie = polozenie.Substring(0, polozenie.IndexOf(":")-1);
                var mpaidlist = Task.Run(() => orderDetailVm.GetTwrKartyAsync(order.kod)).Result;
                var mpa= mpaidlist.Where(s => s.Polozenie == zam.twrkarty.Polozenie).FirstOrDefault();
                zam.twrkarty.MgA_Id = mpa.MgA_Id;
            }
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

            var odp = await RodzajeMetod.TylkoWyjdz(orderDetailVm._orderid);
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
