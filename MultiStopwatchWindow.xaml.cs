using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.Win32;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;
using MultiStopwatch.ViewModels;
using Application = System.Windows.Application;
using PowerLineStatus = System.Windows.Forms.PowerLineStatus;

namespace MultiStopwatch;

public partial class MultiStopwatchWindow : Window
{
    public static string AppName => "MultiStopwatch";
    public static string ActiveWindowsRegPath => @$"SOFTWARE\{AppName}\ActiveWindows";
    public Stopwatch FirstStopwatch { get; set; }
    public Stopwatch SecondStopwatch { get; set; }
    public MainViewModel ViewModel { get; set; }
    public StopwatchWindow StopwatchWindow { get; set; }

    public MultiStopwatchWindow(StopwatchWindow stopwatchWindow)
    {
        InitializeComponent();
        Closed += OnClosing;
        NotifyIcon.TrayLeftMouseDown += CtxToggleBothBtn_OnClick;
        NotifyIcon.ContextMenu.Closed += NotifyIconOnContextMenuClosed;
        SystemEvents.PowerModeChanged += PowerModeChanged;

        StopwatchWindow = stopwatchWindow;
        FirstStopwatch = new Stopwatch(FirstStopwatchTextBox);
        SecondStopwatch = new Stopwatch(SecondStopwatchTextBox);

        ViewModel = new MainViewModel();
        DataContext = ViewModel;

        RestoreWindowPosition();
        RestoreWindowsScales();
    }

    private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        switch (e.Mode)
        {
            case PowerModes.StatusChange:
                // The power status has changed (e.g., plugged in or unplugged)
                if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline)
                {
                    // Power outage detected
                    // You can take appropriate actions here
                    // For example, display a message or save application state
                }
                break;
        }
    }

    private void CtxSettingsBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(this, StopwatchWindow);
        settingsWindow.Show();
    }

    public void SetWindowsScales(double scale)
    {
        ViewModel.Scale = scale;
        StopwatchWindow.ViewModel.Scale = scale;
    }

    public void RestoreWindowsScales()
    {
        var scale = RegHelper.RestoreWindowScale();
        SetWindowsScales(scale * 0.01);
    }

    private void NotifyIconOnContextMenuClosed(object sender, RoutedEventArgs e)
    {
        StopwatchWindow.ResetTopMost();
        ResetTopMost();
    }

    public void RefreshContextMenuItems()
    {
        ViewModel.StopwatchCheckBox = RegHelper.IsWindowActive(AppWindow.Stopwatch);

        ViewModel.MultiStopwatchCheckBox = RegHelper.IsWindowActive(AppWindow.MultiStopwatch);

        ViewModel.StartupCheckBox = RegHelper.IsStartupEnabled();

        if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 1 ||
            RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 1)
        {
            ViewModel.ToggleBothBtnEnabled = true;
            ViewModel.ToggleBothBtnLabel = "Hide All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("HideDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 ||
                 RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0)
        {
            ViewModel.ToggleBothBtnEnabled = true;
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
                 !RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0)
        {
            ViewModel.ToggleBothBtnEnabled = false;
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
    }

    private void OnClosing(object? o, EventArgs eventArgs)
    {
        SaveWindowPosition();
        NotifyIcon.Dispose();
    }

    private void SaveWindowPosition()
    {
        RegHelper.SaveWindowPosition(AppWindow.MultiStopwatch, Left, Top);
    }

    private void RestoreWindowPosition()
    {
        var winPos = RegHelper.RestoreWindowPosition(AppWindow.MultiStopwatch, Width, Height);
        Left = winPos.Left;
        Top = winPos.Top;
    }

    private void ResetButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResetStopwatches();
    }

    public void ResetStopwatches()
    {
        FirstStopwatch.Reset();
        SecondStopwatch.Reset();
        StartButton.Click -= StartButton_OnClick;
        StartButton.Click -= SwitchButton_OnClick;
        StartButton.Click += StartButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("StartDrawingImage");
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        FirstStopwatch.Start();
        StartButton.Click -= StartButton_OnClick;
        StartButton.Click += SwitchButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("SwitchDrawingImage");
    }

    private void SwitchButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (FirstStopwatch.IsRunning)
        {
            FirstStopwatch.Pause();
            SecondStopwatch.Resume();
            StartBtnIcon.Source = (ImageSource)FindResource("FlippedSwitchDrawingImage");
        }
        else if (SecondStopwatch.IsRunning)
        {
            SecondStopwatch.Pause();
            FirstStopwatch.Resume();
            StartBtnIcon.Source = (ImageSource)FindResource("SwitchDrawingImage");
        }
    }


    private void CtxToggleBothBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleBothBtn();
        RefreshContextMenuItems();
    }

    public void ResetTopMost()
    {
        Topmost = false;
        Topmost = true;
    }

    private void ToggleBothBtn()
    {
        if (Opacity == 1 && StopwatchWindow.Opacity == 1)
        {
            Opacity = 0;
            StopwatchWindow.Opacity = 0;
            ResetTopMost();
            StopwatchWindow.ResetTopMost();
        }
        else if (Opacity == 1)
        {
            Opacity = 0;
            ResetTopMost();
            ViewModel.ToggleBothBtnLabel = "Show All";
        }
        else if (StopwatchWindow.Opacity == 1)
        {
            StopwatchWindow.Opacity = 0;
            StopwatchWindow.ResetTopMost();
            ViewModel.ToggleBothBtnLabel = "Show All";
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
                 RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0)
        {
            Opacity = 1;
            StopwatchWindow.Opacity = 1;
            ResetTopMost();
            StopwatchWindow.ResetTopMost();
            ViewModel.ToggleBothBtnLabel = "Show All";
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0)
        {
            Opacity = 1;
            ResetTopMost();
            ViewModel.ToggleBothBtnLabel = "Show All";
        }
        else if (RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0)
        {
            StopwatchWindow.Opacity = 1;
            StopwatchWindow.ResetTopMost();
            ViewModel.ToggleBothBtnLabel = "Show All";
        }
        else if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
                 !RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0)
        {
            ViewModel.ToggleBothBtnEnabled = false;
            ViewModel.ToggleBothBtnLabel = "Show All";
        }
    }

    private void CtxExitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}