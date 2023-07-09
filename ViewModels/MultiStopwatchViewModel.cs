using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using MultiStopwatch.Annotations;

namespace MultiStopwatch.ViewModels;

public class MultiStopwatchViewModel : INotifyPropertyChanged
{
    private string _toggleStopwatchBtnLabel;
    public string ToggleStopwatchBtnLabel
    {
        get => _toggleStopwatchBtnLabel;
        set
        {
            _toggleStopwatchBtnLabel = value;
            OnPropertyChanged(nameof(ToggleStopwatchBtnLabel));
        }
    }

    public ImageSource _toggleStopwatchBtnIcon { get; set; }
    public ImageSource ToggleStopwatchBtnIcon
    {
        get => _toggleStopwatchBtnIcon;
        set
        {
            _toggleStopwatchBtnIcon = value;
            OnPropertyChanged(nameof(ToggleStopwatchBtnIcon));
        }
    }

    private string _toggleMultiStopwatchBtnLabel;
    public string ToggleMultiStopwatchBtnLabel
    {
        get => _toggleMultiStopwatchBtnLabel;
        set
        {
            _toggleMultiStopwatchBtnLabel = value;
            OnPropertyChanged(nameof(ToggleMultiStopwatchBtnLabel));
        }
    }

    public ImageSource _toggleMultiStopwatchBtnIcon { get; set; }
    public ImageSource ToggleMultiStopwatchBtnIcon
    {
        get => _toggleMultiStopwatchBtnIcon;
        set
        {
            _toggleMultiStopwatchBtnIcon = value;
            OnPropertyChanged(nameof(ToggleMultiStopwatchBtnIcon));
        }
    }

    private string _toggleBothBtnLabel;
    public string ToggleBothBtnLabel
    {
        get => _toggleBothBtnLabel;
        set
        {
            _toggleBothBtnLabel = value;
            OnPropertyChanged(nameof(ToggleBothBtnLabel));
        }
    }

    public ImageSource _toggleBothBtnIcon { get; set; }
    public ImageSource ToggleBothBtnIcon
    {
        get => _toggleBothBtnIcon;
        set
        {
            _toggleBothBtnIcon = value;
            OnPropertyChanged(nameof(ToggleBothBtnIcon));
        }
    }

    private bool _toggleBothBtnEnabled = true;
    public bool ToggleBothBtnEnabled
    {
        get => _toggleBothBtnEnabled;
        set
        {
            _toggleBothBtnEnabled = value;
            OnPropertyChanged(nameof(ToggleBothBtnEnabled));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}