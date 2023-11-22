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
    private bool _isInitializing = true;

    public SettingsWindow(MultiStopwatchWindow multiStopwatchWindow, StopwatchWindow stopwatchWindow)
    {
        InitializeComponent();

        Loaded += OnLoaded;

        _multiStopwatchWindow = multiStopwatchWindow;
        _stopwatchWindow = stopwatchWindow;

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
            ToggleStopwatchBtn();
    }

    private void MultiStopwatchCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
            ToggleMultiStopwatchBtn();
    }

    private void StartupCheckbox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_isInitializing)
            ToggleStartup();
    }

    public void ToggleStopwatchBtn()
    {
        if (RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            _stopwatchWindow.ResetStopwatch();
            _stopwatchWindow.Hide();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.Stopwatch, "OFF");
        }
        else
        {
            if (IsVisible)
                _stopwatchWindow.Show();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.Stopwatch, "ON");
        }
    }

    public void ToggleMultiStopwatchBtn()
    {
        if (RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
        {
            _multiStopwatchWindow.ResetStopwatches();
            _multiStopwatchWindow.Hide();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.MultiStopwatch, "OFF");
        }
        else
        {
            if (_stopwatchWindow.IsVisible)
                _multiStopwatchWindow.Show();
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.MultiStopwatch, "ON");
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