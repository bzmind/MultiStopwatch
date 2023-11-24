using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MultiStopwatch.Utility;

namespace MultiStopwatch;

public partial class SettingsWindow : Window
{
    private readonly MultiStopwatchWindow _multiStopwatchWindow;
    private readonly StopwatchWindow _stopwatchWindow;
    private readonly PomodoroWindow _pomodoroWindow;
    private bool _isInitializing = true;

    public SettingsWindow(MultiStopwatchWindow multiStopwatchWindow, StopwatchWindow stopwatchWindow,
        PomodoroWindow pomodoroWindow)
    {
        InitializeComponent();
        Loaded += OnLoaded;

        _multiStopwatchWindow = multiStopwatchWindow;
        _stopwatchWindow = stopwatchWindow;
        _pomodoroWindow = pomodoroWindow;

        _multiStopwatchWindow.Closed += OnMultiStopwatchAppClosing;

        DataContext = _multiStopwatchWindow.ViewModel;
    }

    public void OnMultiStopwatchAppClosing(object? o, EventArgs eventArgs)
    {
        RegHelper.SaveWindowScale(Convert.ToInt32(ScaleSlider.Value));
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isInitializing = false;
    }

    private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ScaleInput.Text = ScaleSlider.Value.ToString();
        if (_multiStopwatchWindow != null)
        {
            _multiStopwatchWindow.SetWindowsScales(ScaleSlider.Value * 0.01);
        }
    }

    private void ScaleInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Check if the entered text is a numeric value
        if (!int.TryParse(e.Text, out int result))
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
        if (int.TryParse(ScaleInput.Text, out int newValue) && ScaleSlider != null)
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
            ToggleCheckBox(AppWindow.Stopwatch);
    }

    private void MultiStopwatchCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
            ToggleCheckBox(AppWindow.MultiStopwatch);
    }

    private void PomodoroCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
            ToggleCheckBox(AppWindow.Pomodoro);
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
            }
            else if (window == AppWindow.Stopwatch)
            {
                _stopwatchWindow.ResetStopwatch();
                _stopwatchWindow.Hide();
            }
            else
            {
                _pomodoroWindow.ResetStopwatches();
                _pomodoroWindow.Hide();
            }
            RegHelper.SaveWindowOnOrOffInReg(window, "OFF");
        }
        else
        {
            if (window == AppWindow.MultiStopwatch && !_multiStopwatchWindow.IsVisible)
                _multiStopwatchWindow.Show();
            else if (window == AppWindow.Stopwatch && !_stopwatchWindow.IsVisible)
                _stopwatchWindow.Show();
            else if (window == AppWindow.Pomodoro && !_pomodoroWindow.IsVisible)
                _pomodoroWindow.Show();

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
}