using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace CompletOrder.Models
{
    [XmlType("Table")]
    public class TwrKarty: INotifyPropertyChanged
    {
         
        public string Kolor { get; set; }
        public string TwrUrl { get; set; }

        public string Rozmiar { get; set; }

        public int TwrGIDNumer { get; set; }
        public int MgA_Id { get; set; }

        public string MgA_Segment1 { get; set; }

        public string MgA_Segment2 { get; set; }

        public string MgA_Segment3 { get; set; }

        public string TwrKod { get; set; }

        public string TwrNazwa { get; set; }

        public int MgA_MgOId { get; set; }

        private string polozenie;

        public string Polozenie
        {
            get { return polozenie; }
            set { SetValue(ref polozenie, value); }
        }

        public string TwrStan { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;

            OnPropertyChanged(propertyName);


            // OnPropertyChanged(propertyName);
        }
    }
}
