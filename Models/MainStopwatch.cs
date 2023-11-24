using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MultiStopwatch.Models;

public class MainStopwatch
{
    private DateTime StartTime { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    private TimeSpan ElapsedTimeTillPause { get; set; }
    public DispatcherTimer Timer { get; set; }
    private TextBox TextBox { get; set; }
    private bool IsFirstStart { get; set; } = true;
    public bool IsRunning { get; set; }

    public MainStopwatch(TextBox textBox)
    {
        Timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        Timer.Tick += OnTick;
        TextBox = textBox;
    }

    private void OnTick(object? o, EventArgs e)
    {
        if (IsRunning)
        {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
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
        if (IsFirstStart)
        {
            Timer.Start();
            StartTime = DateTime.Now;
            IsRunning = true;
        }
        else
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
        UpdateStopwatch();
    }

    public void Pause()
    {
        Timer.Stop();
        ElapsedTimeTillPause = DateTime.Now - StartTime;
        IsRunning = false;
        IsFirstStart = false;
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