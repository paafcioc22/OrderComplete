using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace CompletOrder.Models
{
    [System.Xml.Serialization.XmlType("Table")]
    public class Presta : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int ZaN_GIDNumer { get; set; }
        public string ZaN_FormaNazwa { get; set; }
        public string ZaN_StatusPlatnosc { get; set; }
        public string ZaN_DokumentObcy { get; set; }
        public string ZaN_SpDostawy { get; set; }
        public string ZaN_DataWystawienia { get; set; }
        public string ZaN_DataRealizacji { get; set; }
        public string ZaN_Stan { get; set; }
        public string KnA_Akronim { get; set; }
        public decimal WartoscZam { get; set; } 
        public string ZaE_TwrNazwa { get; set; }
        public string ZaE_TwrKod { get; set; }
        public int ZaE_Ilosc { get; set; }
        public int ElementId { get; set; } 
        
        private bool isfinish;

        public bool IsFinish
        {
            get { return isfinish; }
            set 
            { 
                isfinish = value;
                SetValue(ref isfinish, value);
                OnPropertyChanged(nameof(IsFinish));
            }
            
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetValue<T>(ref T backingStore, T value,
           [CallerMemberName] string propertyName = "",
           Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public string Color { get; set; }
        public string Kolor { get; set; }
        public string Rozmiar { get; set; }

    }
}
