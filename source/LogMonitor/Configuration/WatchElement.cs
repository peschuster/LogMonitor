using System.Configuration;

namespace LogMonitor.Configuration
{
    public class WatchElement : ConfigurationElement, IWatchConfiguration
    {
        /// <summary>
        /// The XML name of the <see cref="Path"/> property.
        /// </summary>        
        internal const string PathPropertyName = "path";

        /// <summary>
        /// The XML name of the <see cref="Type"/> property.
        /// </summary>
        internal const string TypePropertyName = "type";

        /// <summary>
        /// The XML name of the <see cref="Filter"/> property.
        /// </summary>
        internal const string FilterPropertyName = "filter";

        /// <summary>
        /// The XML name of the <see cref="BufferTime"/> property.
        /// </summary>
        internal const string BufferTimePropertyName = "bufferTime";

        /// <summary>
        /// The XML name of the <see cref="BufferTime"/> property.
        /// </summary>
        internal const string IntervalTimePropertyName = "intervalTime";

        /// <summary>
        /// The XML name of the <see cref="MaxDaysInactive"/> property.
        /// </summary>
        internal const string MaxDaysInactivePropertyName = "maxDaysInactive";

        /// <summary>
        /// Gets or sets the Path.
        /// </summary>        
        [ConfigurationPropertyAttribute(PathPropertyName, IsRequired = true, IsKey = true)]
        public string Path
        {
            get { return (string)base[PathPropertyName]; }
            set { base[PathPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        [ConfigurationPropertyAttribute(TypePropertyName, IsRequired = false)]
        public string Type
        {
            get { return (string)base[TypePropertyName]; }
            set { base[TypePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Filter.
        /// </summary>
        [ConfigurationPropertyAttribute(FilterPropertyName, IsRequired = false, DefaultValue = "*")]
        public string Filter
        {
            get { return (string)base[FilterPropertyName]; }
            set { base[FilterPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the BufferTime.
        /// </summary>
        [ConfigurationPropertyAttribute(BufferTimePropertyName, IsRequired = false, DefaultValue = 500)]
        public int BufferTime
        {
            get { return (int)base[BufferTimePropertyName]; }
            set { base[BufferTimePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the IntervalTime.
        /// </summary>
        [ConfigurationPropertyAttribute(IntervalTimePropertyName, IsRequired = false, DefaultValue = 5000)]
        public int IntervalTime
        {
            get { return (int)base[IntervalTimePropertyName]; }
            set { base[IntervalTimePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the MaxDaysInactive.
        /// </summary>
        [ConfigurationPropertyAttribute(MaxDaysInactivePropertyName, IsRequired = false, DefaultValue = -1)]
        public int MaxDaysInactive
        {
            get { return (int)base[MaxDaysInactivePropertyName]; }
            set { base[MaxDaysInactivePropertyName] = value; }
        }
    }
}