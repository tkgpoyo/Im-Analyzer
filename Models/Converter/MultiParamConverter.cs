using System.Globalization;
using System;
using System.Windows.Data;

namespace Im_Analyzer.Models.Converter
{
    public class MultiParamConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }
        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}