using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Windows.Graphics;
using WinRT;

namespace WinUI3Utilities;

public static class AppHelper
{
    private static MicaController? _backdropController;

    private static WindowsSystemDispatcherQueueHelper? _dispatcherQueueHelper;

    private static SystemBackdropConfiguration? _systemBackdropConfiguration;

    public static async Task BeforeLaunch()
    {
        // AppInstance.GetCurrent().Activated += (_, arguments) => ActivationRegistrar.Dispatch(arguments);
        // InitializeComponent();
        if (AppInstance.GetCurrent().GetActivatedEventArgs().Kind is ExtendedActivationKind.ToastNotification)
            return;

        var isProtocolActivated = AppInstance.GetCurrent().GetActivatedEventArgs() is { Kind: ExtendedActivationKind.Protocol };
        if (isProtocolActivated && AppInstance.GetInstances().Count > 1)
        {
            var notCurrent = AppInstance.GetInstances().First(ins => !ins.IsCurrent);
            await notCurrent.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
        }
    }

    /// <summary>
    /// Are we Windows 11 or not?
    /// Windows 11 starts with 10.0.22000
    /// </summary>
    public static bool IsWindows11() => Environment.OSVersion.Version.Build >= 22000;

    /// <summary>
    /// Calculate the window size by current resolution
    /// </summary>
    public static SizeInt32 PredetermineEstimatedWindowSize() =>
        UIHelper.GetScreenSize() switch
        {
            ( >= 2560, >= 1440) => new(1600, 900),
            ( > 1600, > 900) => new(1280, 720),
            _ => new(800, 600)
        };

    public static void TryApplyMica()
    {
        if (MicaController.IsSupported())
        {
            _dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
            _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

            _systemBackdropConfiguration = new SystemBackdropConfiguration();
            CurrentContext.Window.Activated += WindowOnActivated;
            CurrentContext.Window.Closed += WindowOnClosed;
            ((FrameworkElement)CurrentContext.Window.Content).ActualThemeChanged += OnActualThemeChanged;

            _systemBackdropConfiguration.IsInputActive = true;
            SetConfigurationSourceTheme();

            _backdropController = new MicaController();

            _ = _backdropController.AddSystemBackdropTarget(CurrentContext.Window.As<ICompositionSupportsSystemBackdrop>());
            _backdropController.SetSystemBackdropConfiguration(_systemBackdropConfiguration);
        }
    }

    private static void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (_systemBackdropConfiguration is not null)
            SetConfigurationSourceTheme();
    }

    private static void SetConfigurationSourceTheme() =>
        _systemBackdropConfiguration!.Theme = CurrentContext.Window.Content switch
        {
            FrameworkElement { ActualTheme: ElementTheme.Dark } => SystemBackdropTheme.Dark,
            FrameworkElement { ActualTheme: ElementTheme.Light } => SystemBackdropTheme.Light,
            FrameworkElement { ActualTheme: ElementTheme.Default } => SystemBackdropTheme.Default,
            _ => _systemBackdropConfiguration!.Theme
        };

    private static void WindowOnClosed(object sender, WindowEventArgs args)
    {
        if (_backdropController is not null)
        {
            _backdropController.Dispose();
            _backdropController = null;
        }

        CurrentContext.Window.Activated -= WindowOnActivated;
        _systemBackdropConfiguration = null;
    }

    private static void WindowOnActivated(object sender, WindowActivatedEventArgs args)
        => _systemBackdropConfiguration!.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;

    public static void InitializeAsync(SizeInt32 size)
    {
        RegisterUnhandledExceptionHandler();

        CurrentContext.AppWindow.Title = CurrentContext.Title;
        CurrentContext.AppWindow.Resize(size);
        CurrentContext.AppWindow.Show();
        if (CurrentContext.IconPath is "")
            CurrentContext.AppWindow.SetIcon(CurrentContext.IconPath);

        TitleBarHelper.InitializeTitleBar();
        TryApplyMica();
    }

    #region DebugHelper

    public static void RegisterUnhandledExceptionHandler()
    {
        CurrentContext.App.UnhandledException += async (_, args) =>
        {
            args.Handled = true;
            await CurrentContext.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        TaskScheduler.UnobservedTaskException += async (_, args) =>
        {
            args.SetObserved();
            await CurrentContext.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(args.Exception));
        };

        AppDomain.CurrentDomain.UnhandledException += async (_, args) =>
        {
            if (args.ExceptionObject is Exception e)
            {
                await CurrentContext.Window.DispatcherQueue.EnqueueAsync(async () => await UncaughtExceptionHandler(e));
            }
            else
            {
                ExitWithPushedNotification();
            }
        };

#if DEBUG
        // ReSharper disable once UnusedParameter.Local
        static Task UncaughtExceptionHandler(Exception e)
        {
            Debugger.Break();
            return Task.CompletedTask;
        }
#elif RELEASE
            Task UncaughtExceptionHandler(Exception e)
            {
                return ShowExceptionDialogAsync(e);
            }
#endif
    }
    /// <summary>
    ///     Exit the notification after pushing an <see cref="ApplicationExitingMessage" />
    ///     to the <see cref="EventChannel" />
    /// </summary>
    /// <returns></returns>
    public static void ExitWithPushedNotification()
    {
        _ = WeakReferenceMessenger.Default.Send(new ApplicationExitingMessage());
        Application.Current.Exit();
    }

    public record ApplicationExitingMessage;

    #endregion
}
