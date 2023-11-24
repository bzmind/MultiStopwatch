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
    public MainStopwatch FirstMainStopwatch { get; set; }
    public MainViewModel ViewModel { get; set; }

    public StopwatchWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => ResetTopMost();
        Closed += OnClosing;

        FirstMainStopwatch = new MainStopwatch(FirstStopwatchTextBox);

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
        FirstMainStopwatch.Start();
        StartBtn.Click -= StartBtn_OnClick;
        StartBtn.Click += PauseButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("PauseDrawingImage");
        SetBorderPadding(4); // The icon was a bit large, I added padding to make it smaller
    }

    private void PauseButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (FirstMainStopwatch.IsRunning)
        {
            FirstMainStopwatch.Pause();
            StartBtnIcon.Source = (ImageSource)FindResource("StartDrawingImage");
            SetBorderPadding(3.5);
        }
        else
        {
            FirstMainStopwatch.Start();
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
        FirstMainStopwatch.Reset();
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