using Microsoft.Win32;
using System;
using System.Windows;

namespace MultiStopwatch.Utility;

public static class RegHelper
{
    public static string AppName => "MultiStopwatch";
    public static string ActiveWindowsRegPath => @$"SOFTWARE\{AppName}\ActiveWindows";
    private static string WindowPositionRegPath => @$"SOFTWARE\{AppName}\Position";
    private static string WindowScaleRegPath => @$"SOFTWARE\{AppName}\Scale";

    public static void SaveWindowOnOrOffInReg(AppWindow window, string onOrOff)
    {
        var key = Registry.CurrentUser.CreateSubKey(ActiveWindowsRegPath);

        if (window == AppWindow.Stopwatch)
            key.SetValue(nameof(AppWindow.Stopwatch), onOrOff == "ON" ? "ON" : "OFF");
        else if (window == AppWindow.MultiStopwatch)
            key.SetValue(nameof(AppWindow.MultiStopwatch), onOrOff == "ON" ? "ON" : "OFF");
        else
            key.SetValue(nameof(AppWindow.Pomodoro), onOrOff == "ON" ? "ON" : "OFF");

        key.Close();
    }

    public static bool IsWindowActive(AppWindow window)
    {
        var key = Registry.CurrentUser.OpenSubKey(ActiveWindowsRegPath);

        if (key == null)
            return true;

        if (window == AppWindow.Stopwatch)
        {
            var isStopwatchOn = key.GetValue(nameof(AppWindow.Stopwatch));

            if (isStopwatchOn == null)
                return true;

            key.Close();

            return isStopwatchOn.ToString() == "ON";
        }

        if (window == AppWindow.MultiStopwatch)
        {
            var isMultiStopwatchOn = key.GetValue(nameof(AppWindow.MultiStopwatch));

            if (isMultiStopwatchOn == null)
                return true;

            key.Close();

            return isMultiStopwatchOn.ToString() == "ON";
        }

        var isPomodoroOn = key.GetValue(nameof(AppWindow.Pomodoro));

        if (isPomodoroOn == null)
            return true;

        key.Close();

        return isPomodoroOn.ToString() == "ON";
    }

    public static void SaveWindowPosition(AppWindow window, double left, double top)
    {
        var key = Registry.CurrentUser.CreateSubKey(WindowPositionRegPath);

        if (window == AppWindow.MultiStopwatch)
        {
            key.SetValue("MultiStopwatch_Left", left);
            key.SetValue("MultiStopwatch_Top", top);
        }
        else if (window == AppWindow.Stopwatch)
        {
            key.SetValue("Stopwatch_Left", left);
            key.SetValue("Stopwatch_Top", top);
        }
        else
        {
            key.SetValue("Pomodoro_Left", left);
            key.SetValue("Pomodoro_Top", top);
        }

        key.Close();
    }

    public static WinPos RestoreWindowPosition(AppWindow window, double width, double height)
    {
        var key = Registry.CurrentUser.OpenSubKey(WindowPositionRegPath);
        var winPos = new WinPos();

        if ((window == AppWindow.MultiStopwatch
             && (key == null
                 || key.GetValue("MultiStopwatch_Left") == null
                 || key.GetValue("MultiStopwatch_Top") == null))
            || (window == AppWindow.Stopwatch
                && (key == null
                    || key.GetValue("Stopwatch_Left") == null
                    || key.GetValue("Stopwatch_Top") == null))
            || (window == AppWindow.Pomodoro
                && (key == null
                    || key.GetValue("Pomodoro_Left") == null
                    || key.GetValue("Pomodoro_Top") == null)))
        {
            winPos.Left = (SystemParameters.PrimaryScreenWidth - width) / 2;
            winPos.Top = (SystemParameters.PrimaryScreenHeight - height) / 2;
        }
        else
        {
            double left;
            double top;

            if (window == AppWindow.MultiStopwatch)
            {
                left = Convert.ToDouble(key.GetValue("MultiStopwatch_Left"));
                top = Convert.ToDouble(key.GetValue("MultiStopwatch_Top"));
            }
            else if (window == AppWindow.Stopwatch)
            {
                left = Convert.ToDouble(key.GetValue("Stopwatch_Left"));
                top = Convert.ToDouble(key.GetValue("Stopwatch_Top"));
            }
            else
            {
                left = Convert.ToDouble(key.GetValue("Pomodoro_Left"));
                top = Convert.ToDouble(key.GetValue("Pomodoro_Top"));
            }

            winPos.Left = left;
            winPos.Top = top;
        }

        key?.Close();

        return winPos;
    }

    public static bool IsStartupEnabled()
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

        var appPath = Environment.ProcessPath;
        var registryValue = registryKey?.GetValue(AppName);

        return registryValue != null && registryValue.ToString() == appPath;
    }

    public static void EnableRunAtStartup()
    {
        var appPath = Environment.ProcessPath;

        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        if (appPath != null && registryKey?.GetValue(AppName) != appPath)
        {
            registryKey?.SetValue(AppName, appPath);
        }
    }

    public static void DisableRunAtStartup()
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        registryKey?.DeleteValue(AppName, false);
    }

    public static void SaveWindowScale(int scale)
    {
        var key = Registry.CurrentUser.CreateSubKey(WindowScaleRegPath);
        key.SetValue("Scale", scale, RegistryValueKind.String);
        key.Close();
    }

    public static int RestoreWindowScale()
    {
        var key = Registry.CurrentUser.OpenSubKey(WindowScaleRegPath);
        int _scale;

        if (key == null || key.GetValue("Scale") == null)
            _scale = 100;
        else
            _scale = Convert.ToInt32(key.GetValue("Scale"));

        key?.Close();
        return _scale;
    }
}

public enum AppWindow
{
    Stopwatch,
    MultiStopwatch,
    Pomodoro
}