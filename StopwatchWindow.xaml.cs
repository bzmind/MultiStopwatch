using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;

namespace MultiStopwatch;

public partial class StopwatchWindow : Window
{
    public Stopwatch FirstStopwatch { get; set; }

    public StopwatchWindow()
    {
        InitializeComponent();
        Closed += OnClosing;
        FirstStopwatch = new Stopwatch(FirstStopwatchTextBox);
        RestoreWindowPosition();
    }

    private void OnClosing(object? o, EventArgs eventArgs)
    {
        SaveWindowPosition();
    }

    private void SaveWindowPosition()
    {
        RegHelper.SaveWindowPosition(AppWindow.Stopwatch, Left, Top);
    }

    private void RestoreWindowPosition()
    {
        var winPos = RegHelper.RestoreWindowPosition(AppWindow.Stopwatch, Width, Height);
        Left = winPos.Left;
        Top = winPos.Top;
    }

    private void StartBtn_OnClick(object sender, RoutedEventArgs e)
    {
        FirstStopwatch.Start();
        StartBtn.Click -= StartBtn_OnClick;
        StartBtn.Click += PauseButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("PauseDrawingImage");
        SetBorderPadding(4);
    }

    private void PauseButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (FirstStopwatch.IsRunning)
        {
            FirstStopwatch.Pause();
            StartBtnIcon.Source = (ImageSource)FindResource("StartDrawingImage");
            SetBorderPadding(3.5);
        }
        else
        {
            FirstStopwatch.Resume();
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
        FirstStopwatch.Reset();
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