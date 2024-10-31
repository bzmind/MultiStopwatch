using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;
using MultiStopwatch.ViewModels;
using Application = System.Windows.Application;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;

namespace MultiStopwatch;

public partial class MultiStopwatchWindow : AbstractWindow
{
    public MainStopwatch GreenStopwatch { get; set; }
    public MainStopwatch RedStopwatch { get; set; }
    public MainViewModel ViewModel { get; set; }
    public StopwatchWindow StopwatchWindow { get; set; }
    public PomodoroWindow PomodoroWindow { get; set; }
    public NotifyIcon NotifyIcon { get; set; }

    private readonly ToolStripMenuItem _toggleAllMenuItem;

    // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
    // what value of the enum to set.
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern long DwmSetWindowAttribute(IntPtr hwnd,
        DWMWINDOWATTRIBUTE attribute,
        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
        uint cbAttribute);

    public MultiStopwatchWindow(StopwatchWindow stopwatchWindow, PomodoroWindow pomodoroWindow)
    {
        InitializeComponent();
        Loaded += (_, _) => ResetTopMost();
        Closed += OnClosing;
        SystemEvents.PowerModeChanged += PowerModeChanged<MultiStopwatchWindow>;

        NotifyIcon = new NotifyIcon
        {
            Icon = Properties.Resources.Stopwatch,
            Visible = true,
            Text = "MultiStopwatch"
        };

        NotifyIcon.MouseClick += TrayToggleAll_OnClick;

        var contextMenu = new ContextMenuStrip();
        //contextMenu.AutoSize = false;
        //contextMenu.Width = 100;
        //contextMenu.Height = 91;
        contextMenu.ShowImageMargin = false;
        contextMenu.ForeColor = Color.White;
        contextMenu.Renderer = new RoundedRenderer();
        contextMenu.Opening += (_, _) =>
        {
            System.Drawing.Point mousePosition = System.Windows.Forms.Cursor.Position;
            mousePosition.Offset(0, -contextMenu.Height);
            contextMenu.Show(mousePosition);
        };

        var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL;
        DwmSetWindowAttribute(contextMenu.Handle, attribute, ref preference, sizeof(uint));

        _toggleAllMenuItem = new ToolStripMenuItem("Show All");
        _toggleAllMenuItem.Click += ToggleAll_OnClick;

        var settingsMenuItem = new ToolStripMenuItem("Settings");
        settingsMenuItem.Click += Settings_OnClick;

        var resetPositionMenuItem = new ToolStripMenuItem("Reset Position");
        resetPositionMenuItem.Click += ResetPosition_OnClick;

        var exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += Exit_OnClick;

        contextMenu.Items.Add(_toggleAllMenuItem);
        contextMenu.Items.Add(settingsMenuItem);
        contextMenu.Items.Add(resetPositionMenuItem);
        contextMenu.Items.Add(exitMenuItem);

        NotifyIcon.ContextMenuStrip = contextMenu;

        PomodoroWindow = pomodoroWindow;
        StopwatchWindow = stopwatchWindow;
        GreenStopwatch = new MainStopwatch(GreenStopwatchTextBox);
        RedStopwatch = new MainStopwatch(RedStopwatchTextBox);

        ViewModel = new MainViewModel();
        DataContext = ViewModel;

        RestoreWindowPosition(AppWindow.MultiStopwatch);
        RestoreWindowsScales();
    }

    private void Settings_OnClick(object? sender, EventArgs e)
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
            _toggleAllMenuItem.Text = "Hide All";
        }
        else if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0
                 || RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0
                 || RegHelper.IsWindowActive(AppWindow.Pomodoro) && PomodoroWindow.Opacity == 0)
        {
            ViewModel.IsToggleAllEnabled = true;
            _toggleAllMenuItem.Text = "Show All";
        }
        else if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
                 !RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0 &&
                 !RegHelper.IsWindowActive(AppWindow.Pomodoro) && PomodoroWindow.Opacity == 0)
        {
            ViewModel.IsToggleAllEnabled = false;
            _toggleAllMenuItem.Text = "Show All";
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

    private void TrayToggleAll_OnClick(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ToggleAll();
            RefreshContextMenuItems();
        }
    }

    private void ToggleAll_OnClick(object? sender, EventArgs e)
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
            _toggleAllMenuItem.Text = "Show All";
        }
        else if (Opacity == 1 && RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
        {
            Opacity = 0;
            ResetTopMost();
            _toggleAllMenuItem.Text = "Hide All";
        }

        if (StopwatchWindow.Opacity == 0 && RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            StopwatchWindow.Opacity = 1;
            StopwatchWindow.ResetTopMost();
            _toggleAllMenuItem.Text = "Show All";
        }
        else if (StopwatchWindow.Opacity == 1 && RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            StopwatchWindow.Opacity = 0;
            StopwatchWindow.ResetTopMost();
            _toggleAllMenuItem.Text = "Hide All";
        }

        if (PomodoroWindow.Opacity == 0 && RegHelper.IsWindowActive(AppWindow.Pomodoro))
        {
            PomodoroWindow.Opacity = 1;
            PomodoroWindow.ResetTopMost();
            _toggleAllMenuItem.Text = "Show All";
        }
        else if (PomodoroWindow.Opacity == 1 && RegHelper.IsWindowActive(AppWindow.Pomodoro))
        {
            PomodoroWindow.Opacity = 0;
            PomodoroWindow.ResetTopMost();
            _toggleAllMenuItem.Text = "Hide All";
        }

        if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) && Opacity == 0 &&
            !RegHelper.IsWindowActive(AppWindow.Stopwatch) && StopwatchWindow.Opacity == 0 &&
            !RegHelper.IsWindowActive(AppWindow.Pomodoro) && PomodoroWindow.Opacity == 0)
        {
            ViewModel.IsToggleAllEnabled = false;
            _toggleAllMenuItem.Text = "Show All";
        }
    }

    private void ResetPosition_OnClick(object? sender, EventArgs e)
    {
        RestoreWindowPosition(AppWindow.MultiStopwatch);
        StopwatchWindow.RestoreWindowPosition(AppWindow.Stopwatch);
        PomodoroWindow.RestoreWindowPosition(AppWindow.Pomodoro);
    }

    private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs mouseEventArgs)
    {
        MainBorder.Background = new SolidColorBrush(Colors.Black); // Change to black on hover
    }

    private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs mouseEventArgs)
    {
        MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 255, 255, 255)); // Back to transparent
    }

    private readonly HashSet<Key> _pressedKeys = new();

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (!_pressedKeys.Contains(e.Key))
            _pressedKeys.Add(e.Key);

        MoveWindow();
    }

    private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
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

    private void Exit_OnClick(object? sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }
}