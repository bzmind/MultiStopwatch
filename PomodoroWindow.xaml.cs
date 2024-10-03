using System;
using System.Windows;
using MultiStopwatch.Models;
using MultiStopwatch.Utility;
using System.Windows.Media;
using MultiStopwatch.ViewModels;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Input;

namespace MultiStopwatch;

public partial class PomodoroWindow : AbstractWindow
{
    public MainViewModel ViewModel { get; set; }
    public MainStopwatch GreenStopwatch { get; set; }
    public MainStopwatch RedStopwatch { get; set; }
    public enum PomodoroState { Off, Study, Break, LongBreak }
    public PomodoroState State { get; set; } = PomodoroState.Off;

    private const double StudyDuration = 25 * 60;
    private const double BreakDuration = 5 * 60;
    private const double LongBreakDuration = 20 * 60;

    private int _studySessions;
    private DateTime _shortBreakStart;
    private DateTime _longBreakStart;
    private int _amountOfShortBreakTillNow;
    private int _amountOfLongBreakTillNow;

    private double _lastValidStudyNumber;
    private double _lastValidBreakNumber;
    private double _lastValidLongBreakNumber;

    public PomodoroWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => ResetTopMost();
        Closed += OnClosing;
        SystemEvents.PowerModeChanged += PowerModeChanged<PomodoroWindow>;

        GreenStopwatch = new MainStopwatch(GreenStopwatchTextBox);
        RedStopwatch = new MainStopwatch(RedStopwatchTextBox);

        RestoreWindowPosition(AppWindow.Pomodoro);

        ViewModel = new MainViewModel();
        DataContext = ViewModel;
    }

    public void OnObserverTimerTick(object? o, EventArgs e)
    {
        if (State == PomodoroState.Study)
        {
            var elapsedSeconds = GreenStopwatch.ElapsedTime;
            var division = elapsedSeconds / StudyDuration.TruncateDecimal(2);
            var roundedDivision = Math.Round(elapsedSeconds / StudyDuration);
            var isRepetitive = elapsedSeconds == _lastValidStudyNumber;
            var isItTimeToBreak = elapsedSeconds != 0 && division == roundedDivision
                                                      && isRepetitive == false && division >= 0.1;

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

            var division = amount / BreakDuration.TruncateDecimal(2);
            var roundedDivision = Math.Round(amount / BreakDuration);
            var isItTimeToStudy = amount != 0 && division == roundedDivision
                                              && isRepetitive == false && division >= 0.1
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
            var division = amount / LongBreakDuration.TruncateDecimal(2);
            var roundedDivision = Math.Round(amount / LongBreakDuration);
            var isItTimeToStudy = amount != 0 && division == roundedDivision
                                              && isRepetitive == false && division >= 0.1
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