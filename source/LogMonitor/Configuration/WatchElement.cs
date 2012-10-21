using System.Configuration;

namespace LogMonitor.Configuration
{
    public class WatchElement : ConfigurationElement
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
        [ConfigurationPropertyAttribute(FilterPropertyName, IsRequired = false)]
        public string Filter
        {
            get { return (string)base[FilterPropertyName]; }
            set { base[FilterPropertyName] = value; }
        }
    }
}
