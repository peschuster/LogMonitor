namespace LogMonitor.Configuration
{
    public interface IWatchConfiguration
    {
        /// <summary>
        /// Gets the Path.
        /// </summary>        
        string Path { get; }

        /// <summary>
        /// Gets the Type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the Filter.
        /// </summary>
        string Filter { get; }

        /// <summary>
        /// Gets the BufferTime.
        /// </summary>
        int BufferTime { get; }

        int IntervalTime { get; }

        int MaxDaysInactive { get; }
    }
}