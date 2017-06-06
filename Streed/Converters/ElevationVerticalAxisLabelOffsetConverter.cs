using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Streed.Converters
{
    public class ElevationVerticalAxisLabelOffsetConverter : DependencyObject, IValueConverter
    {
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset",
                                        typeof(double),
                                        typeof(ElevationVerticalAxisLabelOffsetConverter),
                                        new PropertyMetadata(0.0));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    var elevation = System.Convert.ToDouble(value);
                    var label = elevation - Offset;
                    label = Math.Round(label, 0, MidpointRounding.AwayFromZero);
                    return label;
                }
                catch
                { }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
