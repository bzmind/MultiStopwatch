using System.Windows;
using MultiStopwatch.Utility;

namespace MultiStopwatch;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var stopwatchWindow = new StopwatchWindow();
        var multiStopwatchWindow = new MultiStopwatchWindow(stopwatchWindow);

        if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
            multiStopwatchWindow.Hide();
        else
            multiStopwatchWindow.Show();

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