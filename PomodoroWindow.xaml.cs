using System;
using System.Windows;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;
using System.Windows.Media;
using MultiStopwatch.ViewModels;

namespace MultiStopwatch;

public partial class PomodoroWindow : AbstractWindow
{
    public MainViewModel ViewModel { get; set; }
    public MainStopwatch GreenStopwatch { get; set; }
    public MainStopwatch RedStopwatch { get; set; }
    public enum PomodoroState { Off, Study, Break, LongBreak }
    public PomodoroState State { get; set; } = PomodoroState.Off;

    private const double StudyDuration = 3;
    private const double BreakDuration = 3;
    private const double LongBreakDuration = 6;

    private int _studySessions;
    private DateTime _shortBreakStart;
    private DateTime _longBreakStart;
    private int _amountOfShortBreakTillNow;
    private int _amountOfLongBreakTillNow;

    private int _lastValidStudyNumber;
    private int _lastValidBreakNumber;
    private int _lastValidLongBreakNumber;

    public PomodoroWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => ResetTopMost();
        Closed += OnClosing;

        GreenStopwatch = new MainStopwatch(GreenStopwatchTextBox);
        RedStopwatch = new MainStopwatch(RedStopwatchTextBox);

        RestoreWindowPosition(AppWindow.Pomodoro);

        ViewModel = new MainViewModel();
        DataContext = ViewModel;
    }

    public void OnObserverTimerTick(object? o, EventArgs e)
    {
        Console.WriteLine(GreenStopwatch.GetElapsedTime());
        if (State == PomodoroState.Study)
        {
            var elapsedSeconds = (int)GreenStopwatch.ElapsedTime.TotalSeconds;
            var division = TruncateDecimal(elapsedSeconds / StudyDuration, 1);
            var roundedDivision = Math.Round(elapsedSeconds / StudyDuration);
            var isRepetitive = elapsedSeconds == _lastValidStudyNumber;
            var isItTimeToBreak = elapsedSeconds != 0 && division == roundedDivision && isRepetitive == false;

            if (isItTimeToBreak && _studySessions == 3)
            {
                _lastValidStudyNumber = elapsedSeconds;
                _longBreakStart = DateTime.Now;
                GreenStopwatch.Pause();
                RedStopwatch.Start();
                State = PomodoroState.LongBreak;
                return;
            }

            if (isItTimeToBreak)
            {
                _lastValidStudyNumber = elapsedSeconds;
                _shortBreakStart = DateTime.Now;
                GreenStopwatch.Pause();
                RedStopwatch.Start();
                State = PomodoroState.Break;
            }
        }
        else if (State == PomodoroState.Break)
        {
            var elapsedSeconds = (int)(DateTime.Now - _shortBreakStart).TotalSeconds;
            int amount;
            bool isRepetitive;
            if (_amountOfShortBreakTillNow == 0)
            {
                amount = elapsedSeconds;
                isRepetitive = elapsedSeconds == _lastValidBreakNumber;
            }
            else
            {
                amount = _amountOfShortBreakTillNow + elapsedSeconds;
                isRepetitive = amount == _lastValidBreakNumber;
            }
            var division = TruncateDecimal(amount / BreakDuration, 1);
            var roundedDivision = Math.Round(amount / BreakDuration);
            var isItTimeToStudy = amount != 0 && division == roundedDivision && isRepetitive == false
                                  && elapsedSeconds != 0;

            if (isItTimeToStudy)
            {
                _lastValidBreakNumber = elapsedSeconds;
                RedStopwatch.Pause();
                GreenStopwatch.Start();
                _amountOfShortBreakTillNow += elapsedSeconds;
                _studySessions++;
                State = PomodoroState.Study;
            }
        }
        else
        {
            var elapsedSeconds = (int)(DateTime.Now - _longBreakStart).TotalSeconds;
            int amount;
            bool isRepetitive;
            if (_amountOfLongBreakTillNow == 0)
            {
                amount = elapsedSeconds;
                isRepetitive = elapsedSeconds == _lastValidLongBreakNumber;
            }
            else
            {
                amount = _amountOfLongBreakTillNow + elapsedSeconds;
                isRepetitive = amount == _lastValidLongBreakNumber;
            }
            var division = TruncateDecimal(amount / LongBreakDuration, 1);
            var roundedDivision = Math.Round(amount / LongBreakDuration);
            var isItTimeToStudy = amount != 0 && division == roundedDivision && isRepetitive == false
                && elapsedSeconds != 0;

            if (isItTimeToStudy)
            {
                _lastValidLongBreakNumber = elapsedSeconds;
                RedStopwatch.Pause();
                GreenStopwatch.Start();
                _studySessions = 0;
                _amountOfLongBreakTillNow += elapsedSeconds;
                State = PomodoroState.Study;
            }
        }
    }

    public static double TruncateDecimal(double number, int decimals)
    {
        var factor = Math.Pow(10, decimals);
        return Math.Truncate(number * factor) / factor;
    }

    protected override void OnClosing(object? o, EventArgs eventArgs)
    {
        SaveWindowPosition(AppWindow.Pomodoro);
    }

    private void StartBtn_OnClick(object sender, RoutedEventArgs e)
    {
        GreenStopwatch.Timer.Tick += OnObserverTimerTick;
        RedStopwatch.Timer.Tick += OnObserverTimerTick;
        GreenStopwatch.Start();
        State = PomodoroState.Study;

        StartBtn.Click -= StartBtn_OnClick;
        StartBtn.Click += ResetButton_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("ResetDrawingImage");
    }

    private void ResetButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResetStopwatches();
    }

    public void ResetStopwatches()
    {
        GreenStopwatch.Reset();
        RedStopwatch.Reset();
        StartBtn.Click -= StartBtn_OnClick;
        StartBtn.Click -= ResetButton_OnClick;
        StartBtn.Click += StartBtn_OnClick;
        StartBtnIcon.Source = (ImageSource)FindResource("StartDrawingImage");

        _studySessions = default;
        _shortBreakStart = default;
        _longBreakStart = default;
        _amountOfShortBreakTillNow = default;
        _amountOfLongBreakTillNow = default;
        _lastValidStudyNumber = default;
        _lastValidBreakNumber = default;
        _lastValidLongBreakNumber = default;
    }
}