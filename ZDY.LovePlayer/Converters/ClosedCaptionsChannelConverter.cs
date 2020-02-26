using System;
using System.Globalization;
using System.Windows.Data;
using Unosquare.FFME.ClosedCaptions;

namespace ZDY.LovePlayer.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class ClosedCaptionsChannelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (CaptionsChannel)value != CaptionsChannel.CCP;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? CaptionsChannel.CC1 : CaptionsChannel.CCP;
        }
    }
}
