﻿using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;
using MultiStopwatch.ViewModels;
using Application = System.Windows.Application;

namespace MultiStopwatch;

public partial class MultiStopwatchWindow : AbstractWindow
{
    public MainStopwatch GreenStopwatch { get; set; }
    public MainStopwatch RedStopwatch { get; set; }
    public MainViewModel ViewModel { get; set; }
    public StopwatchWindow StopwatchWindow { get; set; }
    public PomodoroWindow PomodoroWindow { get; set; }

    public MultiStopwatchWindow(StopwatchWindow stopwatchWindow, PomodoroWindow pomodoroWindow)
    {
        InitializeComponent();
        Loaded += (_, _) => ResetTopMost();
        Closed += OnClosing;
        SystemEvents.PowerModeChanged += PowerModeChanged<MultiStopwatchWindow>;

        NotifyIcon.TrayLeftMouseDown += ToggleAll_OnClick;
        NotifyIcon.ContextMenu.Closed += NotifyIconOnContextMenuClosed;

        PomodoroWindow = pomodoroWindow;
        StopwatchWindow = stopwatchWindow;
        GreenStopwatch = new MainStopwatch(GreenStopwatchTextBox);
        RedStopwatch = new MainStopwatch(RedStopwatchTextBox);

        ViewModel = new MainViewModel();
        DataContext = ViewModel;

        RestoreWindowPosition(AppWindow.MultiStopwatch);
        RestoreWindowsScales();
    }

    private void Settings_OnClick(object sender, RoutedEventArgs e)
    {
        if (SettingsWindow.IsOpen) return;
        var settingsWindow = new SettingsWindow(this, StopwatchWindow, PomodoroWindow);
        settingsWindow.Show();
    }

    public void SetWindowsScales(double scale)
    {
        ViewModel.Scale = scale.TruncateDecimal(4);
        StopwatchWindow.ViewModel.Scale = scale.TruncateDecimal(4);
        PomodoroWindow.ViewModel.Scale = scale.TruncateDecimal(4);
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
        ViewModel.PomodoroCheckBox = RegHelper.IsWindowActive(AppWindow.Pomodoro);
        ViewModel.StartupCheckBox = RegHelper.IsStartupEnabled();

        if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 1
            || RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 1
            || RegHelper.IsWindowActive(AppWindow.Pomodoro) && PomodoroWindow.Opacity == 1)
        {
            ViewModel.IsToggleAllEnabled = true;
            ViewModel.ToggleAllLabelText = "Hide All";
            ViewModel.ToggleAllIcon = (ImageSource)FindResource("HideDrawingImage");
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0
                 || RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0
                 || RegHelper.IsWindowActive(AppWindow.Pomodoro) && PomodoroWindow.Opacity == 0)
        {
            ViewModel.IsToggleAllEnabled = true;
            ViewModel.ToggleAllLabelText = "Show All";
            ViewModel.ToggleAllIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
        else if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
                 !RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0 &&
                 !RegHelper.IsWindowActive(AppWindow.Pomodoro) && PomodoroWindow.Opacity == 0)
        {
            ViewModel.IsToggleAllEnabled = false;
            ViewModel.ToggleAllLabelText = "Show All";
            ViewModel.ToggleAllIcon = (ImageSource)FindResource("ShowDrawingImage");
        }
    }

    protected override void OnClosing(object? o, EventArgs eventArgs)
    {
        SaveWindowPosition(AppWindow.MultiStopwatch);
        NotifyIcon.Dispose();
    }

    private void ResetButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResetStopwatches();
    }

    public void ResetStopwatches()
    {
        GreenStopwatch.Reset();
        RedStopwatch.Reset();
        StartButton.Click -= StartButton_OnClick;
        StartButton.Click -= SwitchButton_OnClick;
        StartButton.Click += StartButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("StartDrawingImage");
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        GreenStopwatch.Start();
        StartButton.Click -= StartButton_OnClick;
        StartButton.Click += SwitchButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("SwitchDrawingImage");
    }

    private void SwitchButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (GreenStopwatch.IsRunning)
        {
            GreenStopwatch.Pause();
            RedStopwatch.Start();
            StartBtnIcon.Source = (ImageSource)FindResource("FlippedSwitchDrawingImage");
        }
        else if (RedStopwatch.IsRunning)
        {
            RedStopwatch.Pause();
            GreenStopwatch.Start();
            StartBtnIcon.Source = (ImageSource)FindResource("SwitchDrawingImage");
        }
    }

    private void ToggleAll_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleAll();
        RefreshContextMenuItems();
    }

    private void ToggleAll()
    {
        if (Opacity == 0 && RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
        {
            Opacity = 1;
            ResetTopMost();
            ViewModel.ToggleAllLabelText = "Show All";
        }
        else if (Opacity == 1 && RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
        {
            Opacity = 0;
            ResetTopMost();
            ViewModel.ToggleAllLabelText = "Hide All";
        }

        if (StopwatchWindow.Opacity == 0 && RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            StopwatchWindow.Opacity = 1;
            StopwatchWindow.ResetTopMost();
            ViewModel.ToggleAllLabelText = "Show All";
        }
        else if (StopwatchWindow.Opacity == 1 && RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            StopwatchWindow.Opacity = 0;
            StopwatchWindow.ResetTopMost();
            ViewModel.ToggleAllLabelText = "Hide All";
        }

        if (PomodoroWindow.Opacity == 0 && RegHelper.IsWindowActive(AppWindow.Pomodoro))
        {
            PomodoroWindow.Opacity = 1;
            PomodoroWindow.ResetTopMost();
            ViewModel.ToggleAllLabelText = "Show All";
        }
        else if (PomodoroWindow.Opacity == 1 && RegHelper.IsWindowActive(AppWindow.Pomodoro))
        {
            PomodoroWindow.Opacity = 0;
            PomodoroWindow.ResetTopMost();
            ViewModel.ToggleAllLabelText = "Hide All";
        }

        if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
            !RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0 &&
            !RegHelper.IsWindowActive(AppWindow.Pomodoro) && PomodoroWindow.Opacity == 0)
        {
            ViewModel.IsToggleAllEnabled = false;
            ViewModel.ToggleAllLabelText = "Show All";
        }
    }

    private void ResetPosition_OnClick(object sender, RoutedEventArgs e)
    {
        RestoreWindowPosition(AppWindow.MultiStopwatch);
        StopwatchWindow.RestoreWindowPosition(AppWindow.Stopwatch);
        PomodoroWindow.RestoreWindowPosition(AppWindow.Pomodoro);
    }

    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}