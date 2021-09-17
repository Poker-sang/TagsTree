using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
    public static class ObservableCollectionHelper
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> iEnumerable)
        {
            var observableCollection = new ObservableCollection<T>();
            foreach (var item in iEnumerable)
                observableCollection.Add(item);
            return observableCollection;
        }
    }
}