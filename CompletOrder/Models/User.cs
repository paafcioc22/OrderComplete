using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CompletOrder.Models
{
    [XmlType("Table")]
    public class User
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool VisiblePass { get; set; }
    }
}
