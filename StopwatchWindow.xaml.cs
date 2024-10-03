using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
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
        SystemEvents.PowerModeChanged += PowerModeChanged<StopwatchWindow>;

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

    private void Border_MouseEnter(object sender, MouseEventArgs mouseEventArgs)
    {
        MainBorder.Background = new SolidColorBrush(Colors.Black); // Change to black on hover
    }

    private void Border_MouseLeave(object sender, MouseEventArgs mouseEventArgs)
    {
        MainBorder.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255)); // Back to transparent
    }

    private readonly HashSet<Key> _pressedKeys = new();

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (!_pressedKeys.Contains(e.Key))
            _pressedKeys.Add(e.Key);

        MoveWindow();
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
        if (_pressedKeys.Contains(e.Key))
            _pressedKeys.Remove(e.Key);
    }

    private void MoveWindow()
    {
        if (_pressedKeys.Contains(Key.Left))
            Left -= 1;

        if (_pressedKeys.Contains(Key.Right))
            Left += 1;

        if (_pressedKeys.Contains(Key.Up))
            Top -= 1;

        if (_pressedKeys.Contains(Key.Down))
            Top += 1;
    }
}