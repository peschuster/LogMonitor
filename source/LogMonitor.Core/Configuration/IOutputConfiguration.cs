namespace LogMonitor.Configuration
{
    public interface IOutputConfiguration
    {
        /// <summary>
        /// Gets or sets the PathPattern.
        /// </summary>        
        string PathPattern { get; }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets or sets the Target.
        /// </summary>
        string Target { get; }

        /// <summary>
        /// Gets or sets the MetricsPrefix.
        /// </summary>
        string MetricsPrefix { get; }
    }
}