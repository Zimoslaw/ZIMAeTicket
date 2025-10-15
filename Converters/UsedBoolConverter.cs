using System;
using System.Collections.Generic;
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ZIMAeTicket.Converters
{
    internal class UsedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool used)
                return "NaN";

            return used ? "Wykorzystany" : "Niewykorzystany";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
