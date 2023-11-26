using System;

namespace MultiStopwatch.Utility;

public static class Utils
{
    public static double TruncateDecimal(this double number, int decimals)
    {
        var factor = Math.Pow(10, decimals);
        return Math.Truncate(number * factor) / factor;
    }
}