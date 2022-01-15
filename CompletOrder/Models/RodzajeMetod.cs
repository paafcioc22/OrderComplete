using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CompletOrder.Models
{
    public class RodzajeMetod
    {
        public static SendOrder orderSended;
        static string deviceIdentifier;
        public RodzajeMetod()
        {
            IDevice device = DependencyService.Get<IDevice>();
            deviceIdentifier = device.GetIdentifier();
        }



        public static async Task<bool> WejdżWZamowienie(int orderId, string orderData)
        {
            bool zwroc=true;

            IDevice device = DependencyService.Get<IDevice>();
            deviceIdentifier = device.GetIdentifier();

            var send = new SendOrder()
            {
                Orn_OrderId = orderId,
                Orn_IsDone = false,
                Orn_IsEdit = true,
                Orn_DoneUser = 0,
                Orn_EditUser = 0,
                Orn_OrderData = orderData,
                Orn_DoneData = null,
                Orn_DeviceId = deviceIdentifier
            };

            var odp = await App.TodoManager.InsertOrderSend(send);
            if (odp != "OK")
                zwroc = false;


            return zwroc;
        }

        public static async Task<bool> ZakonczIwyjdz(int orderId, string doneData)
        {
            bool zwroc = true;

            var login = Preferences.Get("user", "default_value");

            IDevice device = DependencyService.Get<IDevice>();
            deviceIdentifier = device.GetIdentifier();

            var send = new SendOrder()
            {
                Orn_OrderId = orderId,
                Orn_IsDone = true,
                Orn_IsEdit = false,
                Orn_DoneUser = 0,
                Orn_EditUser = 0,                
                Orn_DoneData = doneData,
                Orn_DeviceId = deviceIdentifier,
                Orn_UsrLogin = login
            };

            var odp = await App.TodoManager.InsertOrderSend(send);
            if (odp != "OK")
                zwroc = false;


            return zwroc;
        }


        public static async Task<bool> TylkoWyjdz(int orderId )
        {
            bool zwroc = true;

            IDevice device = DependencyService.Get<IDevice>();
            deviceIdentifier = device.GetIdentifier();

            var send = new SendOrder()
            {
                Orn_OrderId = orderId,
                Orn_IsDone = false,
                Orn_IsEdit = false,
                Orn_DoneUser = 0,
                Orn_EditUser = 0,
                Orn_DeviceId = ""
                 
            };

            var odp = await App.TodoManager.InsertOrderSend(send);
            if (odp != "OK")
                zwroc = false;


            return zwroc;
        }





    }
}
