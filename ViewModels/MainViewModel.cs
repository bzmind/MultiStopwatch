using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using MultiStopwatch.Properties;

namespace MultiStopwatch.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public bool _stopwatchCheckBox { get; set; }
    public bool StopwatchCheckBox
    {
        get => _stopwatchCheckBox;
        set
        {
            _stopwatchCheckBox = value;
            OnPropertyChanged(nameof(StopwatchCheckBox));
        }
    }

    public bool _multiStopwatchCheckBox { get; set; }
    public bool MultiStopwatchCheckBox
    {
        get => _multiStopwatchCheckBox;
        set
        {
            _multiStopwatchCheckBox = value;
            OnPropertyChanged(nameof(MultiStopwatchCheckBox));
        }
    }

    public bool _pomodoroCheckBox { get; set; }
    public bool PomodoroCheckBox
    {
        get => _pomodoroCheckBox;
        set
        {
            _pomodoroCheckBox = value;
            OnPropertyChanged(nameof(PomodoroCheckBox));
        }
    }

    public bool _startupCheckBox { get; set; }
    public bool StartupCheckBox
    {
        get => _startupCheckBox;
        set
        {
            _startupCheckBox = value;
            OnPropertyChanged(nameof(StartupCheckBox));
        }
    }

    private string _toggleAllLabelText;
    public string ToggleAllLabelText
    {
        get => _toggleAllLabelText;
        set
        {
            _toggleAllLabelText = value;
            OnPropertyChanged(nameof(ToggleAllLabelText));
        }
    }

    public ImageSource _toggleAllIcon { get; set; }
    public ImageSource ToggleAllIcon
    {
        get => _toggleAllIcon;
        set
        {
            _toggleAllIcon = value;
            OnPropertyChanged(nameof(ToggleAllIcon));
        }
    }

    private bool _isIsToggleAllEnabled = true;
    public bool IsToggleAllEnabled
    {
        get => _isIsToggleAllEnabled;
        set
        {
            _isIsToggleAllEnabled = value;
            OnPropertyChanged(nameof(IsToggleAllEnabled));
        }
    }

    private double _scale = 1.0;
    public double Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                _scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
    }

    private int _scaleInput;
    public int ScaleInput
    {
        get => Convert.ToInt32(Scale * 100);
        set
        {
            if (_scaleInput != value)
            {
                _scaleInput = value;
                OnPropertyChanged(nameof(ScaleInput));
            }
        }
    }

    private int _scaleSlider;
    public int ScaleSlider
    {
        get => Convert.ToInt32(Scale * 100);
        set
        {
            if (_scaleSlider != value)
            {
                _scaleSlider = value;
                OnPropertyChanged(nameof(ScaleSlider));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}