using CompletOrder.Models;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CompletOrder.ViewModels
{
    public class OrderDetail : INotifyPropertyChanged
    {

        //ObservableCollection<OrderDetail> OrderDetailVM;

        //private ObservableCollection<OrderDetail> _orderDetailVM;

        //public ObservableCollection<OrderDetail> OrderDetailVM
        //{
        //    get { return _orderDetailVM; }
        //    set { 
        //        var tmp = new ObservableCollection<OrderDetail>(value.OrderBy(x => x.IsDone).ThenBy(x => x.twrkarty.MgA_Segment1).ThenBy(x => x.twrkarty.MgA_Segment2).ThenBy(x => x.twrkarty.MgA_Segment3));
        //        _orderDetailVM= Convert2(tmp.ToList());
        //        OnPropertyChanged(nameof(OrderDetailVM));
        //    }
        //}

        
        
         
        public int IdElement { get; set; }
        public int OrderId { get; set; } 
        public TwrKarty twrkarty { get; set; }
        public double cena_netto { get; set; }
        public int ilosc { get; set; }
        public string kod { get; set; }
        public string nazwa { get; set; }
        public string promo { get; set; }
        public string kolor { get; set; }
        public string rozmiar { get; set; }
        public int vat { get; set; } 

        private bool _isDone;
         
        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                if (_isDone == value)
                    return;

                _isDone = value;

                //SetValue(ref _isDone, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(Color));


                //var tmp=OrderDetailVM.OrderBy(x => x.twrkarty.MgA_Segment1).ThenBy(x => x.twrkarty.MgA_Segment2).ThenBy(x => x.twrkarty.MgA_Segment3);
                //OrderDetailVM.orderDetail = Convert2(tmp.ToList());
                //OrderDetailVM = new ObservableCollection<OrderDetail>(Convert2(tmp.ToList()));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public  ObservableCollection<T> Convert2<T>(IList<T> original)
        {
            return new ObservableCollection<T>(original);
        }

        public Color Color
        {
            get { return IsDone ? Color.Green : Color.Black; }
        }

        public string nazwaShort { get; internal set; }
    }
}