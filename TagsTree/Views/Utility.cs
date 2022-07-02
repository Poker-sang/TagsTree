using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace TagsTree.Views;

public class Utility
{
    public static void BackBClick(object sender, RoutedEventArgs e) => App.RootFrame.GoBack(new SlideNavigationTransitionInfo());
}