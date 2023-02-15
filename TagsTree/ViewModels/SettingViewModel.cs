using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities.Attributes;

namespace TagsTree.ViewModels;

[SettingsViewModel<AppConfig>(nameof(AppConfig))]
public partial class SettingViewModel : ObservableObject
{
    public bool ConfigSet => Directory.Exists(LibraryPath);

    public bool IsFileObserverItemEnabled => AppConfig.FilesObserverEnabled && ConfigSet;

    /// <summary>
    /// <see cref="ToggleSwitch.Toggled"/>事件响应时还没有改变绑定的值，所以直接在绑定值里调用事件
    /// </summary>
    public bool FilesObserverEnabled
    {
        get => AppConfig.FilesObserverEnabled;
        set
        {
            _ = SetProperty(AppConfig.FilesObserverEnabled, value, AppConfig, (setting, value) => setting.FilesObserverEnabled = value);
            OnPropertyChanged(nameof(IsFileObserverItemEnabled));
        }
    }

    public void ConfigSetChanged()
    {
        OnPropertyChanged(nameof(ConfigSet));
        OnPropertyChanged(nameof(IsFileObserverItemEnabled));
    }

    public AppConfig AppConfig => AppContext.AppConfig;
}
