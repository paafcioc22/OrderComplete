using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CompletOrder.Models
{
    class KolorujIlosci : IValueConverter
    {
        //public T TrueObject { set; get; }

        //public T FalseObject { set; get; }

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
             
        //    if (value != null)
        //    {
        //        var ilosc = value.ToString().Replace("Ilość : ","").Replace(" szt","");
                
                
        //        if (System.Convert.ToInt32(ilosc) > 1)
        //            return TrueObject;
        //        else
        //            return FalseObject;
        //    }
        //    else
        //    return  FalseObject;


        //}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {
                //var ilosc = value.ToString().Replace("Ilość : ", "").Replace(" szt", "");


                if (System.Convert.ToInt32(value) > 1)
                    return true;
                else
                    return false;
            }
            else
                return false;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;// ((T)value).Equals(TrueObject);
        }
    }
}

 