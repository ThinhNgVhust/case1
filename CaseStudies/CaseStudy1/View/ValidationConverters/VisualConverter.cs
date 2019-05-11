using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CaseStudy1.View.ValidationConverters
{
    public class VisualConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double val = 0;
            bool isAngle = double.TryParse(values[0].ToString(), out val);
            bool isLength = double.TryParse(values[1].ToString(), out val);
            if (isLength)
            {
                isLength = val > 0 ? true : false;
            }
            bool isRadius = double.TryParse(values[2].ToString(), out val);
            if (isRadius)
            {
                isRadius = val > 0 ? true : false;
            }
            bool isString = !string.IsNullOrWhiteSpace(values[3].ToString());
            return isAngle && isLength && isRadius && isString;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
