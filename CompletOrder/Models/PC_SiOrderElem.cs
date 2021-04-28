using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CompletOrder.Models
{
	[XmlType("Table")]
	public class PC_SiOrderElem
    {
		public int OrE_Id { get; set; }
		public int OrE_OrderId { get; set; }
		public int OrE_OrderEleId { get; set; }
		public int OrE_Quantity { get; set; }
		public int OrE_MpaId { get; set; }
		public string OrE_PlaceName { get; set; }
		public int OrE_NewTrnGidNumer { get; set; }
		public DateTime OrE_DateTime { get; set; }
	}
}
