using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompletOrder.Models
{
    public class OrderComplete
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int IdOrder { get; set; }
    }
}
