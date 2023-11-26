using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;
using MultiStopwatch.ViewModels;

namespace MultiStopwatch;

public partial class StopwatchWindow : AbstractWindow
{
    public MainStopwatch Stopwatch { get; set; }
    public MainViewModel ViewModel { get; set; }

    public StopwatchWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => ResetTopMost();
        Closed += OnClosing;

        Stopwatch = new MainStopwatch(StopwatchTextBox);

        RestoreWindowPosition(AppWindow.Stopwatch);

        ViewModel = new MainViewModel();
        DataContext = ViewModel;
    }

    protected override void OnClosing(object? o, EventArgs eventArgs)
    {
        SaveWindowPosition(AppWindow.Stopwatch);
    }

    private void StartBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Stopwatch.Start();
        StartBtn.Click -= StartBtn_OnClick;
        StartBtn.Click += PauseButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("PauseDrawingImage");
        SetBorderPadding(4); // The icon was a bit large, I added padding to make it smaller
    }

    private void PauseButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Stopwatch.IsRunning)
        {
            Stopwatch.Pause();
            StartBtnIcon.Source = (ImageSource)FindResource("StartDrawingImage");
            SetBorderPadding(3.5);
        }
        else
        {
            Stopwatch.Start();
            StartBtnIcon.Source = (ImageSource)FindResource("PauseDrawingImage");
            SetBorderPadding(4);
        }
    }

    private void ResetButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResetStopwatch();
    }

    public void ResetStopwatch()
    {
        Stopwatch.Reset();
        StartBtn.Click -= StartBtn_OnClick;
        StartBtn.Click -= PauseButton_OnClick;
        StartBtn.Click += StartBtn_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("StartDrawingImage");
        SetBorderPadding(3.5);
    }

    private void SetBorderPadding(double amount)
    {
        var startBtnBorder = (Border?)StartBtn.Template.FindName("StartBtnBorder", StartBtn);
        if (startBtnBorder != null) startBtnBorder.Padding = new Thickness(amount);
    }
}