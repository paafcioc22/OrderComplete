using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompletOrder.Models
{
    public interface IWebService
    {
        Task<List<TwrKarty>> GetInfos(string query);
        Task<string> InsertOrderSend(SendOrder sendOrder);
        Task<List<SendOrder>> SelectOrderSend(string query3);
    }
}
