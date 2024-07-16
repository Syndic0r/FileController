namespace FileController.Models;
public record ProgressReport
{
    public ProgressReport(double progress, string message)
    {
        Progress = progress;
        Message = message;
    }
    public double Progress { get; }
    public string Message { get; }
}
