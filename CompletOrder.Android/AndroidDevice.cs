using CompletOrder.Droid;
using CompletOrder.Models;
using System;
using Xamarin.Forms;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]
namespace CompletOrder.Droid
{
    public class AndroidDevice : IDevice
    {
        [Obsolete]
        public string GetIdentifier()
        {
            return Secure.GetString(Forms.Context.ContentResolver, Secure.AndroidId);
        }
    }
}