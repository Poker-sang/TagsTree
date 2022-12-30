using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WinUI3Utilities;

public static class Misc
{
    public static T Cast<T>(this object? obj) where T : notnull => (T)(obj ?? throw new InvalidCastException());

    public static T Get<T>(this WeakReference<T?> target, Func<T> @default) where T : class
    {
        if (!target.TryGetTarget(out var value))
            target.SetTarget(value = @default());
        return value;
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> iEnumerable)
    {
        var observableCollection = new ObservableCollection<T>();
        foreach (var item in iEnumerable)
            observableCollection.Add(item);
        return observableCollection;
    }
}
