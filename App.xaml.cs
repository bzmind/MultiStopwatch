using System.Windows;
using MultiStopwatch.Utility;

namespace MultiStopwatch;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var pomodoroWindow = new PomodoroWindow();
        var stopwatchWindow = new StopwatchWindow();
        var multiStopwatchWindow = new MultiStopwatchWindow(stopwatchWindow, pomodoroWindow);

        if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch))
            multiStopwatchWindow.Hide();
        else
            multiStopwatchWindow.Show();

        if (!RegHelper.IsWindowActive(AppWindow.Stopwatch))
            stopwatchWindow.Hide();
        else
            stopwatchWindow.Show();

        if (!RegHelper.IsWindowActive(AppWindow.Pomodoro))
            pomodoroWindow.Hide();
        else
            pomodoroWindow.Show();

        if (!RegHelper.IsWindowActive(AppWindow.MultiStopwatch)
            && !RegHelper.IsWindowActive(AppWindow.Stopwatch)
            && !RegHelper.IsWindowActive(AppWindow.Pomodoro))
        {
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.MultiStopwatch, "ON");
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.Stopwatch, "ON");
            RegHelper.SaveWindowOnOrOffInReg(AppWindow.Pomodoro, "ON");
            multiStopwatchWindow.Show();
            stopwatchWindow.Show();
            pomodoroWindow.Show();
        }

        multiStopwatchWindow.RefreshContextMenuItems();
    }
}