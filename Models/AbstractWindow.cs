using MultiStopwatch.Utility;
using System;
using System.Windows;

namespace MultiStopwatch.Models;

public abstract class AbstractWindow : Window
{
    protected abstract void OnClosing(object? o, EventArgs eventArgs);

    public void SaveWindowPosition(AppWindow window)
    {
        RegHelper.SaveWindowPosition(window, Left, Top);
    }

    public void ResetTopMost()
    {
        Topmost = false;
        Topmost = true;
    }

    public void RestoreWindowPosition(AppWindow window)
    {
        var winPos = RegHelper.RestoreWindowPosition(window, Width, Height);
        Left = winPos.Left;
        Top = winPos.Top;
    }
}