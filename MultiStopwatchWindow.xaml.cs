using System;
using System.Windows;
using System.Windows.Media;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;
using MultiStopwatch.ViewModels;

namespace MultiStopwatch;

public partial class MultiStopwatchWindow : Window
{
    public static string AppName => "MultiStopwatch";
    public static string ActiveWindowsRegPath => @$"SOFTWARE\{AppName}\ActiveWindows";
    public Stopwatch FirstStopwatch { get; set; }
    public Stopwatch SecondStopwatch { get; set; }
    public MultiStopwatchViewModel ViewModel { get; set; }
    public StopwatchWindow StopwatchWindow { get; set; }

    public MultiStopwatchWindow()
    {
        InitializeComponent();
        Closed += OnClosing;
        NotifyIcon.TrayLeftMouseDown += CtxToggleBothBtn_OnClick;

        FirstStopwatch = new Stopwatch(FirstStopwatchTextBox);
        SecondStopwatch = new Stopwatch(SecondStopwatchTextBox);

        RestoreWindowPosition();

        ViewModel = new MultiStopwatchViewModel();
        DataContext = ViewModel;
    }

    public void RefreshContextMenuItems()
    {
        ViewModel.ToggleStopwatchBtnLabel = "Stopwatch";
        ViewModel.ToggleMultiStopwatchBtnLabel = "Multi-Stopwatch";
        ViewModel.ToggleStartupBtnLabel = "Run at Startup";

        if (RegHelper.IsWindowActive(AppWindow.Stopwatch))
            ViewModel.ToggleStopwatchBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        else
            ViewModel.ToggleStopwatchBtnIcon = (ImageSource)FindResource("UncheckedDrawingImage");

        if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
            ViewModel.ToggleMultiStopwatchBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        else
            ViewModel.ToggleMultiStopwatchBtnIcon = (ImageSource)FindResource("UncheckedDrawingImage");

        if (RegHelper.IsStartupEnabled())
            ViewModel.ToggleStartupBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        else
            ViewModel.ToggleStartupBtnIcon = (ImageSource)FindResource("UncheckedDrawingImage");

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

    private void ResetStopwatches()
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

    private void CtxToggleStopwatchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleStopwatchBtn();
        RefreshContextMenuItems();
    }

    private void ToggleStopwatchBtn()
    {
        if (RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            StopwatchWindow.ResetStopwatch();
            StopwatchWindow.Hide();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.Stopwatch, "OFF");
            ViewModel.ToggleStopwatchBtnLabel = "Stopwatch";
            ViewModel.ToggleStopwatchBtnIcon = (ImageSource)FindResource("UncheckedDrawingImage");
        }
        else
        {
            if (IsVisible)
                StopwatchWindow.Show();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.Stopwatch, "ON");
            ViewModel.ToggleStopwatchBtnLabel = "Stopwatch";
            ViewModel.ToggleStopwatchBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        }
    }

    private void CtxToggleMultiStopwatchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleMultiStopwatchBtn();
        RefreshContextMenuItems();
    }

    private void ToggleMultiStopwatchBtn()
    {
        if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
        {
            ResetStopwatches();
            Hide();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.MultiStopwatch, "OFF");
            ViewModel.ToggleMultiStopwatchBtnLabel = "Multi-Stopwatch";
            ViewModel.ToggleMultiStopwatchBtnIcon = (ImageSource)FindResource("UncheckedDrawingImage");
        }
        else
        {
            if (StopwatchWindow.IsVisible)
                Show();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.MultiStopwatch, "ON");
            ViewModel.ToggleMultiStopwatchBtnLabel = "Multi-Stopwatch";
            ViewModel.ToggleMultiStopwatchBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        }
    }

    private void CtxToggleBothBtn_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleBothBtn();
        RefreshContextMenuItems();
    }

    private void ToggleBothBtn()
    {
        if (Opacity == 1 && StopwatchWindow.Opacity == 1)
        {
            StopwatchWindow.Opacity = 0;
            Opacity = 0;
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (Opacity == 1)
        {
            Opacity = 0;
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (StopwatchWindow.Opacity == 1)
        {
            StopwatchWindow.Opacity = 0;
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
                 RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0)
        {
            Opacity = 1;
            StopwatchWindow.Opacity = 1;
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0)
        {
            Opacity = 1;
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0)
        {
            StopwatchWindow.Opacity = 1;
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

    private void CtxExitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void CtxToggleStartupBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (RegHelper.IsStartupEnabled())
        {
            RegHelper.DisableRunAtStartup();
            ViewModel.ToggleStartupBtnIcon = (ImageSource)FindResource("UncheckedDrawingImage");
        }
        else
        {
            RegHelper.EnableRunAtStartup();
            ViewModel.ToggleStartupBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        }
    }
}