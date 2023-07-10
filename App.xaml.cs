using System.Windows;
using Microsoft.Win32;
using MultiStopwatch.Utility;

namespace MultiStopwatch;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var multiStopwatchWindow = new MultiStopwatchWindow();

        if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
            multiStopwatchWindow.Hide();
        else
            multiStopwatchWindow.Show();

        var stopwatchWindow = new StopwatchWindow();
        multiStopwatchWindow.StopwatchWindow = stopwatchWindow;

        if (!RegHelper.IsWindowActive(AppWindow.Stopwatch))
            stopwatchWindow.Hide();
        else
            stopwatchWindow.Show();

        if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch) &&
            !RegHelper.IsWindowActive(AppWindow.Stopwatch))
        {
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.MultiStopwatch, "ON");
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.Stopwatch, "ON");
            multiStopwatchWindow.Show();
            stopwatchWindow.Show();
        }

        multiStopwatchWindow.RefreshContextMenuItems();
    }
}