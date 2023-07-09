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
        FirstStopwatch = new Stopwatch(FirstStopwatchTextBox);
        SecondStopwatch = new Stopwatch(SecondStopwatchTextBox);
        RestoreWindowPosition();
        ViewModel = new MultiStopwatchViewModel();
        DataContext = ViewModel;
    }

    public void RefreshContextMenuItems()
    {
        if (RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            ViewModel.ToggleStopwatchBtnLabel = "Stopwatch";
            ViewModel.ToggleStopwatchBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        }
        else
        {
            ViewModel.ToggleStopwatchBtnLabel = "Stopwatch";
            ViewModel.ToggleStopwatchBtnIcon = (ImageSource)FindResource("CloseDrawingImage");
        }

        if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
        {
            ViewModel.ToggleMultiStopwatchBtnLabel = "Multi-Stopwatch";
            ViewModel.ToggleMultiStopwatchBtnIcon = (ImageSource)FindResource("CheckedDrawingImage");
        }
        else
        {
            ViewModel.ToggleMultiStopwatchBtnLabel = "Multi-Stopwatch";
            ViewModel.ToggleMultiStopwatchBtnIcon = (ImageSource)FindResource("CloseDrawingImage");
        }

        if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && IsVisible ||
            RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.IsVisible)
        {
            ViewModel.ToggleBothBtnEnabled = true;
            ViewModel.ToggleBothBtnLabel = "Hide All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("HideDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && !IsVisible ||
                 RegHelper.IsWindowActive(AppWindow.Stopwatch) && !StopwatchWindow.IsVisible)
        {
            ViewModel.ToggleBothBtnEnabled = true;
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && !IsVisible &&
                 !RegHelper.IsWindowActive(AppWindow.Stopwatch) && !StopwatchWindow.IsVisible)
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
        }
        else if (SecondStopwatch.IsRunning)
        {
            SecondStopwatch.Pause();
            FirstStopwatch.Resume();
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
            ViewModel.ToggleStopwatchBtnIcon = (ImageSource)FindResource("CloseDrawingImage");
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
            ViewModel.ToggleMultiStopwatchBtnIcon = (ImageSource)FindResource("CloseDrawingImage");
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
        if (IsVisible && StopwatchWindow.IsVisible)
        {
            StopwatchWindow.Hide();
            Hide();
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (IsVisible)
        {
            Hide();
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (StopwatchWindow.IsVisible)
        {
            StopwatchWindow.Hide();
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && !IsVisible &&
                 RegHelper.IsWindowActive(AppWindow.Stopwatch) && !StopwatchWindow.IsVisible)
        {
            Show();
            StopwatchWindow.Show();
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && !IsVisible)
        {
            Show();
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.Stopwatch) && !StopwatchWindow.IsVisible)
        {
            StopwatchWindow.Show();
            ViewModel.ToggleBothBtnLabel = "Show All";
            ViewModel.ToggleBothBtnIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && !IsVisible &&
                 !RegHelper.IsWindowActive(AppWindow.Stopwatch) && !StopwatchWindow.IsVisible)
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
}