using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompletOrder.Models
{
    public class OrderDatailComplete
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public int IdElementOrder { get; set; }
    }
}
