using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompletOrder.Models;

namespace CompletOrder.Services
{
    public class MockDataStore : IDataStore<OrderO>
    {
        readonly List<OrderO> items;

        public MockDataStore()
        {
            items = new List<OrderO>()
            {
                new OrderO { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                new OrderO { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                new OrderO { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                new OrderO { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                new OrderO { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                new OrderO { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(OrderO item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(OrderO item)
        {
            var oldItem = items.Where((OrderO arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((OrderO arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<OrderO> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<OrderO>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}