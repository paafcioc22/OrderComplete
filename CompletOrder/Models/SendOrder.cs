using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CompletOrder.Models
{
    //[XmlType("Table")]
    public class SendOrder
    {
        public int Orn_OrderId { get; set; }
        public bool Orn_IsDone { get; set; }
        public bool Orn_IsEdit { get; set; }
        public int Orn_DoneUser { get; set; }
        public int Orn_EditUser { get; set; }
        public string Orn_OrderData { get; set; }
        public string Orn_DoneData { get; set; }
        public string Orn_DeviceId { get; set; }

    }
}
