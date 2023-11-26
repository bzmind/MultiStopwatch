using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;

namespace MultiStopwatch;

public partial class SettingsWindow : AbstractWindow
{
    public static bool IsOpen { get; set; }
    private readonly MultiStopwatchWindow _multiStopwatchWindow;
    private readonly StopwatchWindow _stopwatchWindow;
    private readonly PomodoroWindow _pomodoroWindow;
    private bool _isInitializing = true;

    public SettingsWindow(MultiStopwatchWindow multiStopwatchWindow, StopwatchWindow stopwatchWindow,
        PomodoroWindow pomodoroWindow)
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosing;

        _multiStopwatchWindow = multiStopwatchWindow;
        _stopwatchWindow = stopwatchWindow;
        _pomodoroWindow = pomodoroWindow;

        _multiStopwatchWindow.Closed += OnMultiStopwatchAppClosing;

        DataContext = _multiStopwatchWindow.ViewModel;
    }

    public void OnMultiStopwatchAppClosing(object? o, EventArgs eventArgs)
    {
        RegHelper.SaveWindowScale(ScaleSlider.Value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isInitializing = false;
        IsOpen = true;
    }

    private void ScaleSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ScaleInput.Text = ScaleSlider.Value.ToString("##.#");
        if (_multiStopwatchWindow != null)
            _multiStopwatchWindow.SetWindowsScales(ScaleSlider.Value * 0.01);
    }

    private void ScaleInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Check if the entered text is a numeric value
        if (!double.TryParse(e.Text, out double result))
        {
            e.Handled = true; // Prevent non-numeric input
        }
        else
        {
            // Check if the numeric value is within the desired range (0-100)
            if (result < 0 || result > 100)
            {
                e.Handled = true; // Prevent values outside the range
            }
        }
    }

    private void ScaleInput_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (double.TryParse(ScaleInput.Text, out double newValue) && ScaleSlider != null)
        {
            // Ensure the parsed value is within the slider's range
            if (newValue >= ScaleSlider.Minimum && newValue <= ScaleSlider.Maximum)
            {
                ScaleSlider.Value = newValue;
                _multiStopwatchWindow.SetWindowsScales(ScaleSlider.Value * 0.01);
            }
        }
    }

    private void StopwatchCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
        {
            ToggleCheckBox(AppWindow.Stopwatch);
            _multiStopwatchWindow.RefreshContextMenuItems();
        }
    }

    private void MultiStopwatchCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
        {
            ToggleCheckBox(AppWindow.MultiStopwatch);
            _multiStopwatchWindow.RefreshContextMenuItems();
        }
    }

    private void PomodoroCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
        {
            ToggleCheckBox(AppWindow.Pomodoro);
            _multiStopwatchWindow.RefreshContextMenuItems();
        }
    }

    private void StartupCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
            ToggleStartup();
    }

    public void ToggleCheckBox(AppWindow window)
    {
        if (RegHelper.IsWindowActive(window))
        {
            if (window == AppWindow.MultiStopwatch)
            {
                _multiStopwatchWindow.ResetStopwatches();
                _multiStopwatchWindow.Hide();
                _multiStopwatchWindow.Opacity = 0;
            }
            else if (window == AppWindow.Stopwatch)
            {
                _stopwatchWindow.ResetStopwatch();
                _stopwatchWindow.Hide();
                _stopwatchWindow.Opacity = 0;
            }
            else
            {
                _pomodoroWindow.ResetStopwatches();
                _pomodoroWindow.Hide();
                _pomodoroWindow.Opacity = 0;
            }
            RegHelper.SaveWindowOnOrOffInReg(window, "OFF");
        }
        else
        {
            if (window == AppWindow.MultiStopwatch && !_multiStopwatchWindow.IsVisible)
            {
                _multiStopwatchWindow.Show();
                _multiStopwatchWindow.Opacity = 1;
            }
            else if (window == AppWindow.Stopwatch && !_stopwatchWindow.IsVisible)
            {
                _stopwatchWindow.Show();
                _stopwatchWindow.Opacity = 1;
            }
            else if (window == AppWindow.Pomodoro && !_pomodoroWindow.IsVisible)
            {
                _pomodoroWindow.Show();
                _pomodoroWindow.Opacity = 1;
            }

            RegHelper.SaveWindowOnOrOffInReg(window, "ON");
        }
    }

    public void ToggleStartup()
    {
        if (RegHelper.IsStartupEnabled())
        {
            RegHelper.DisableRunAtStartup();
            _multiStopwatchWindow.ViewModel.StartupCheckBox = false;
        }
        else
        {
            RegHelper.EnableRunAtStartup();
            _multiStopwatchWindow.ViewModel.StartupCheckBox = true;
        }
    }

    protected override void OnClosing(object? o, EventArgs eventArgs)
    {
        IsOpen = false;
    }
}