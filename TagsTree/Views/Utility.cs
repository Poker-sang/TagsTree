using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace TagsTree.Views;

public class Utility
{
    public static void BackClick(object sender, RoutedEventArgs e) => App.RootFrame.GoBack(new SlideNavigationTransitionInfo());
}