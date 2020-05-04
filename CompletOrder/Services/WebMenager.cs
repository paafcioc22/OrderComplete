using CompletOrder.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompletOrder.Services
{
    public class WebMenager
    {
        IWebService soapService;

        public WebMenager(IWebService service)
        {
            soapService = service;
        }

        public Task<List<TwrKarty>> GetDataFromWeb(string query)
        {
            return soapService.GetInfos(query);
        }

        public Task<string> InsertOrderSend(SendOrder insert)
        {
            return soapService.InsertOrderSend(insert);
        }

        public Task<List<SendOrder>> GetOrdersFromWeb(string query2)
        {
            return soapService.SelectOrderSend(query2);
        }
    }
}
