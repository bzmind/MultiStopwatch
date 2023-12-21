using Microsoft.Win32;
using MultiStopwatch.Utility;
using System;
using System.Windows;

namespace MultiStopwatch.Models;

public abstract class AbstractWindow : Window
{
    protected abstract void OnClosing(object? o, EventArgs eventArgs);

    protected void PowerModeChanged<T>(object sender, PowerModeChangedEventArgs e)
    {
        switch (e.Mode)
        {
            case PowerModes.Suspend:
                if (typeof(T) == typeof(MultiStopwatchWindow))
                    SaveWindowPosition(AppWindow.MultiStopwatch);
                else if (typeof(T) == typeof(StopwatchWindow))
                    SaveWindowPosition(AppWindow.Stopwatch);
                else if (typeof(T) == typeof(PomodoroWindow))
                    SaveWindowPosition(AppWindow.Pomodoro);
                break;

            case PowerModes.Resume:
                if (typeof(T) == typeof(MultiStopwatchWindow))
                    RestoreWindowPosition(AppWindow.MultiStopwatch);
                else if (typeof(T) == typeof(StopwatchWindow))
                    RestoreWindowPosition(AppWindow.Stopwatch);
                else if (typeof(T) == typeof(PomodoroWindow))
                    RestoreWindowPosition(AppWindow.Pomodoro);
                break;
        }
    }

    public void SaveWindowPosition(AppWindow window)
    {
        RegHelper.SaveWindowPosition(window, Left, Top);
    }

    public void RestoreWindowPosition(AppWindow window)
    {
        var winPos = RegHelper.RestoreWindowPosition(window, Width, Height);
        Left = winPos.Left;
        Top = winPos.Top;
    }

    public void ResetTopMost()
    {
        Topmost = false;
        Topmost = true;
    }
}