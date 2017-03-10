using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using yk.ConnectFour.Models;

namespace yk.ConnectFour.Utils
{
    public class CounterPartyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var param = parameter as string;
            if (param == null)
                return DependencyProperty.UnsetValue;
            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            var paramValue = Enum.Parse(value.GetType(), param);
            return paramValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var param = parameter as string;
            if (parameter == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(typeof (CounterParty), param);
        }
    }
}