using System;
using Microsoft.UI.Xaml.Data;
using WinUI3Utilities;

namespace TagsTree.Views.Converters;

public class BoolToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => value.To<bool>() ? 1d : 0d;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => value.To<double>() is not 0;
}
