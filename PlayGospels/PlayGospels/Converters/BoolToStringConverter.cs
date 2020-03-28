using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGospels.Converters {
    public class BoolToStringConverter : IValueConverter, IMarkupExtension {
        public string TrueString { get; set; }
        public string FalseString { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var source = (bool)value;
            if (source)
                return TrueString;
            else
                return FalseString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
