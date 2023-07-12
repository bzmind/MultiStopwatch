using Microsoft.Win32;
using System;
using System.Windows;

namespace MultiStopwatch.Utility;

public static class RegHelper
{
    public static string AppName => "MultiStopwatch";
    private static string ActiveWindowsRegPath => MultiStopwatchWindow.ActiveWindowsRegPath;
    private static string StopwatchWinPosRegPath => @$"SOFTWARE\{AppName}\Stopwatch\AppPosition";
    private static string MultiStopwatchWinPosRegPath => @$"SOFTWARE\{AppName}\AppPosition";

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
        RegistryKey? key;

        if (window == AppWindow.MultiStopwatch)
            key = Registry.CurrentUser.CreateSubKey(MultiStopwatchWinPosRegPath);
        else
            key = Registry.CurrentUser.CreateSubKey(StopwatchWinPosRegPath);

        key.SetValue("WindowLeft", left);
        key.SetValue("WindowTop", top);
        key.Close();
    }

    public static WinPos RestoreWindowPosition(AppWindow window, double width, double height)
    {
        RegistryKey? key;
        var winPos = new WinPos();

        if (window == AppWindow.MultiStopwatch)
            key = Registry.CurrentUser.OpenSubKey(MultiStopwatchWinPosRegPath);
        else
            key = Registry.CurrentUser.OpenSubKey(StopwatchWinPosRegPath);

        if (key == null || key.GetValue("WindowLeft") == null || key.GetValue("WindowTop") == null)
        {
            winPos.Left = (SystemParameters.PrimaryScreenWidth - width) / 2;
            winPos.Top = (SystemParameters.PrimaryScreenHeight - height) / 2;
        }
        else
        {
            var left = Convert.ToDouble(key.GetValue("WindowLeft"));
            var top = Convert.ToDouble(key.GetValue("WindowTop"));
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
        if (registryKey?.GetValue(AppName) == null)
        {
            if (appPath != null) registryKey?.SetValue(AppName, appPath);
        }
    }

    public static void DisableRunAtStartup()
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        registryKey?.DeleteValue(AppName, false);
    }
}

public enum AppWindow
{
    Stopwatch,
    MultiStopwatch
}