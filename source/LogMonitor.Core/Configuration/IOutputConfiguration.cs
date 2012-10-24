namespace LogMonitor.Configuration
{
    public interface IOutputConfiguration
    {
        /// <summary>
        /// Gets the PathPattern.
        /// </summary>        
        string PathPattern { get; }

        /// <summary>
        /// Gets the Type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the Target.
        /// </summary>
        string Target { get; }

        /// <summary>
        /// Gets the MetricsPrefix.
        /// </summary>
        string MetricsPrefix { get; }
    }
}