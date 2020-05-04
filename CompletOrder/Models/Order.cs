 
using System;
using System.Collections.Generic;
using System.Text;

namespace CompletOrder.Models
{
    public class Order
    {

        public int Lp { get; set; }
        public int id { get; set; }
        public bool IsFinish { get; set; }
        public bool IsEdit { get; set; }
        public bool FiltrZaplacone { get; set; }
        public string data { get; set; }
        public double do_zaplaty { get; set; }
        public string faktura_adres { get; set; }
        public string faktura_firma { get; set; }
        public string nr_paragonu { get; set; }
        public string platnosc_info { get; set; }
        public double platnosc_karta_podarunkowa { get; set; }
        public double platnosc_koszt { get; set; }
        public double platnosc_punktami { get; set; }
        public string status { get; set; }
        public string typ_platnosci { get; set; }
        public string uwagi { get; set; }
        public double wartosc_netto { get; set; }
        public string wysylka_info { get; set; }
        public double wysylka_koszt { get; set; }
    }
}
