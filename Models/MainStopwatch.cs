using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MultiStopwatch.Models;

public class MainStopwatch
{
    public DateTime StartTime { get; set; }
    public int ElapsedTime { get; set; }
    private TimeSpan ElapsedTimeTillPause { get; set; }
    public DispatcherTimer Timer { get; set; }
    private TextBox TextBox { get; set; }
    private bool IsFirstStart { get; set; } = true;
    public bool IsRunning { get; set; }

    public MainStopwatch(TextBox textBox)
    {
        Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        Timer.Tick += OnTick;
        TextBox = textBox;
    }

    private void OnTick(object? o, EventArgs e)
    {
        if (IsRunning)
        {
            ElapsedTime = (int)(DateTime.Now - StartTime).TotalSeconds;
            UpdateStopwatch();
        }
    }

    private void UpdateStopwatch()
    {
        TextBox.Text = TimeSpan.FromSeconds(ElapsedTime).ToString(@"hh\:mm\:ss");
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
        ElapsedTime = 0;
        Timer.Stop();
        UpdateStopwatch();
        IsFirstStart = true;
        IsRunning = false;
    }
}