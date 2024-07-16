using Spectre.Console;

namespace FileController.Extensions;
public static class ProgressExtensions
{
    public static Progress RefreshRate(this Progress progress, TimeSpan timeSpan)
    {
        progress.RefreshRate = timeSpan;
        return progress;
    }
}
