using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CompletOrder.Models
{
    [System.Xml.Serialization.XmlType("Table")]
    public class Presta
    {
        public int ZaN_GIDNumer { get; set; }
        public string ZaN_FormaNazwa { get; set; }
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
        public bool IsFinish { get; set; }
        public string Color { get; set; }
        public string Kolor { get; set; }
        public string Rozmiar { get; set; }

    }
}
