using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CompletOrder.Models
{
  //  [XmlRoot(ElementName = "Root")]
    [XmlType("Table")]
    public class Allegro
    {
        [XmlElement(ElementName = "Id")]
        public int Id { get; set; }
        public bool IsFinish { get; set; }
        public string CustomerName { get; set; }
        public string  RaportDate { get; set; }
        //public string SellDate { get; set; }
        [XmlElement(ElementName = "Signature")]
        public string kod { get; set; }
        [XmlElement(ElementName = "ProdName")]
        public string nazwa { get; set; }
        [XmlElement(ElementName = "Quantity")]
        public int ilosc { get; set; }
        public string Pol1 { get; set; }
        public string Pol2 { get; set; }
        public string Pol3 { get; set; }
        public string forma_platnosc { get; set; }
        public string NrParagonu { get; set; }
    }
}
