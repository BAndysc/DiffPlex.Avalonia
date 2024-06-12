namespace DiffPlex.Avalonia;

internal static class Utils
{
    public static int Clamp(int value, int min, int max)
    {
        if (value < min)
            return min;
        return value > max ? max : value;
    }

    public static double Clamp(double value, double min, double max)
    {
        if (value < min)
            return min;
        return value > max ? max : value;
    }
}