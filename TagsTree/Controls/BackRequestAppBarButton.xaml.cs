using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace TagsTree.Controls;

public sealed partial class BackRequestAppBarButton : AppBarButton
{
    public BackRequestAppBarButton() => InitializeComponent();
    private void BackBClick(object sender, RoutedEventArgs e) => App.RootFrame.GoBack(new SlideNavigationTransitionInfo());
}