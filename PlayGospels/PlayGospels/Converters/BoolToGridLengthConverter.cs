using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGospels.Converters {
    public class BoolToGridLengthConverter : IValueConverter, IMarkupExtension {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var isTwo = (bool)value;
            if (isTwo) {
                return new GridLength(1d, GridUnitType.Star);
            }
            else {
                return new GridLength(0, GridUnitType.Absolute);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
