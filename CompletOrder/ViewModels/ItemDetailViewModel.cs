using System;

using CompletOrder.Models;

namespace CompletOrder.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public OrderO Item { get; set; }
        public ItemDetailViewModel(OrderO item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
