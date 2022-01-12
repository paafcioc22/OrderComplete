using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CompletOrder.ViewModels
{
    public class ObjectNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IComparable v = value as IComparable;
            if (parameter == null )
            {

                return !string.IsNullOrEmpty((string)value);
            }
            else
            {
                //IComparable v = value as IComparable;
                IComparable p = parameter as IComparable;

                if (v != null && p != null)
                    return Equals(v, p);
                return false;
            }
        }
         
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
