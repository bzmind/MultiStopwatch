using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MultiStopwatch.Models;

public class Stopwatch
{
    private DateTime StartTime { get; set; }
    private DateTime PauseTime { get; set; }
    private TimeSpan ElapsedTime { get; set; }
    private TimeSpan ElapsedTimeTillPause { get; set; }
    private DispatcherTimer Timer { get; set; }
    private TextBox TextBox { get; set; }
    private bool IsFirstStart { get; set; } = true;
    public bool IsRunning { get; set; }

    public Stopwatch(TextBox textBox)
    {
        Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
        Timer.Tick += OnTick;
        TextBox = textBox;
    }

    private void OnTick(object? o, EventArgs e)
    {
        if (IsRunning)
        {
            ElapsedTime = DateTime.Now - StartTime;
            UpdateStopwatch();
        }
    }

    public string GetElapsedTime()
    {
        return ElapsedTime.ToString(@"hh\:mm\:ss");
    }

    private void UpdateStopwatch()
    {
        TextBox.Text = GetElapsedTime();
    }

    public void Start()
    {
        Timer.Start();
        StartTime = DateTime.Now;
        IsRunning = true;
    }

    public void Pause()
    {
        Timer.Stop();
        ElapsedTimeTillPause = DateTime.Now - StartTime;
        IsRunning = false;
        IsFirstStart = false;
    }

    public void Resume()
    {
        if (IsRunning)
            return;

        IsRunning = true;
        StartTime = DateTime.Now - ElapsedTimeTillPause;

        if (IsFirstStart)
            Start();
        else
            Timer.Start();
    }

    public void Reset()
    {
        ElapsedTime = TimeSpan.Zero;
        Timer.Stop();
        UpdateStopwatch();
        IsFirstStart = true;
        IsRunning = false;
    }
}