using Microsoft.Win32;
using System;
using System.Windows;

namespace MultiStopwatch.Utility;

public static class RegHelper
{
    public static string AppName => "MultiStopwatch";
    private static string ActiveWindowsRegPath => MultiStopwatchWindow.ActiveWindowsRegPath;
    private static string WindowPositionRegPath => @$"SOFTWARE\{AppName}\Position";
    private static string WindowScaleRegPath => @$"SOFTWARE\{AppName}\Scale";

    public static void SaveWindowOnOrOffInReg(AppWindow window, string onOrOff)
    {
        if (window == AppWindow.Stopwatch)
        {
            var key = Registry.CurrentUser.CreateSubKey(ActiveWindowsRegPath);
            key.SetValue(nameof(AppWindow.Stopwatch), onOrOff == "ON" ? "ON" : "OFF");
            key.Close();
        }
        else
        {
            var key = Registry.CurrentUser.CreateSubKey(ActiveWindowsRegPath);
            key.SetValue(nameof(AppWindow.MultiStopwatch), onOrOff == "ON" ? "ON" : "OFF");
            key.Close();
        }
    }

    public static bool IsWindowActive(AppWindow window)
    {
        if (window == AppWindow.Stopwatch)
        {
            var key = Registry.CurrentUser.OpenSubKey(ActiveWindowsRegPath);

            if (key == null)
                return true;

            var stopwatchWindow = key.GetValue(nameof(AppWindow.Stopwatch));

            if (stopwatchWindow == null)
                return true;

            key.Close();

            return stopwatchWindow.ToString() == "ON";
        }
        else
        {
            var key = Registry.CurrentUser.OpenSubKey(ActiveWindowsRegPath);

            if (key == null)
                return true;

            var multiStopwatch = key.GetValue(nameof(AppWindow.MultiStopwatch));

            if (multiStopwatch == null)
                return true;

            key.Close();

            return multiStopwatch.ToString() == "ON";
        }
    }

    public static void SaveWindowPosition(AppWindow window, double left, double top)
    {
        RegistryKey key = Registry.CurrentUser.CreateSubKey(WindowPositionRegPath);

        if (window == AppWindow.MultiStopwatch)
        {
            key.SetValue("MultiStopwatch_Left", left);
            key.SetValue("MultiStopwatch_Top", top);
        }
        else
        {
            key.SetValue("Stopwatch_Left", left);
            key.SetValue("Stopwatch_Top", top);
        }

        key.Close();
    }

    public static WinPos RestoreWindowPosition(AppWindow window, double width, double height)
    {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(WindowPositionRegPath);
        var winPos = new WinPos();

        if (window == AppWindow.MultiStopwatch
            && (key == null
            || key.GetValue("MultiStopwatch_Left") == null
            || key.GetValue("MultiStopwatch_Top") == null))
        {
            winPos.Left = (SystemParameters.PrimaryScreenWidth - width) / 2;
            winPos.Top = (SystemParameters.PrimaryScreenHeight - height) / 2;
        }
        else if (window == AppWindow.Stopwatch
            && (key == null
            || key.GetValue("Stopwatch_Left") == null
            || key.GetValue("Stopwatch_Top") == null))
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
            else
            {
                left = Convert.ToDouble(key.GetValue("Stopwatch_Left"));
                top = Convert.ToDouble(key.GetValue("Stopwatch_Top"));
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
        RegistryKey key = Registry.CurrentUser.CreateSubKey(WindowScaleRegPath);
        key.SetValue("Scale", scale, RegistryValueKind.String);
        key.Close();
    }

    public static int RestoreWindowScale()
    {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(WindowScaleRegPath);
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
    MultiStopwatch
}