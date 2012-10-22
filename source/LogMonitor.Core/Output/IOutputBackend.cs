namespace LogMonitor.Output
{
    public interface IOutputBackend
    {
        string[] SupportedTypes { get; }

        void Submit(Metric metric, string metricsPrefix);
    }
}