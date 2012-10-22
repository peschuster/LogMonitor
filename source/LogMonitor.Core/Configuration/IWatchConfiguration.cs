namespace LogMonitor.Configuration
{
    public interface IWatchConfiguration
    {
        /// <summary>
        /// Gets or sets the Path.
        /// </summary>        
        string Path { get; }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets or sets the Filter.
        /// </summary>
        string Filter { get; }

        /// <summary>
        /// Gets or sets the BufferTime.
        /// </summary>
        int BufferTime { get; }
    }
}