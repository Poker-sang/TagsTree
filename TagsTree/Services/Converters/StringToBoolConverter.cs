using System;
using Microsoft.UI.Xaml.Data;
using WinUI3Utilities;

namespace TagsTree.Services.Converters;

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => !string.IsNullOrEmpty(value.To<string>());

    public object ConvertBack(object value, Type targetType, object parameter, string language) => ThrowHelper.InvalidCast<object>();
}
