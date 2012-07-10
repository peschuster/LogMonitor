using System.Configuration;
using LogMonitor.Configuration.Ouput;

namespace LogMonitor.Configuration
{
    public class OutputConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The XML name of the OutputConfigurationSectionName Configuration Section.
        /// </summary>        
        internal const string OutputConfigurationSectionName = "ouput";

        /// <summary>
        /// The XML name of the <see cref="Xmlns"/> property.
        /// </summary>        
        internal const string XmlnsPropertyName = "xmlns";

        /// <summary>
        /// The XML name of the <see cref="Graphite"/> property.
        /// </summary>        
        internal const string GraphitePropertyName = "graphite";

        /// <summary>
        /// The XML name of the <see cref="StatsD"/> property.
        /// </summary>        
        internal const string StatsDPropertyName = "statsd";

        /// <summary>
        /// Gets the W3CReadersConfiguration instance.
        /// </summary>        
        public static OutputConfiguration Instance
        {
            get { return (OutputConfiguration)ConfigurationManager.GetSection(OutputConfigurationSectionName); }
        }

        /// <summary>
        /// Gets the XML namespace of this Configuration Section.
        /// </summary>
        /// <remarks>
        /// This property makes sure that if the configuration file contains the XML namespace,
        /// the parser doesn't throw an exception because it encounters the unknown "xmlns" attribute.
        /// </remarks>        
        [ConfigurationPropertyAttribute(XmlnsPropertyName)]
        public string Xmlns
        {
            get { return (string)this[XmlnsPropertyName]; }
        }

        /// <summary>
        /// Gets or sets the Counters.
        /// </summary>
        [ConfigurationPropertyAttribute(GraphitePropertyName)]
        public GraphiteElement Graphite
        {
            get { return (GraphiteElement)this[GraphitePropertyName]; }
            set { base[GraphitePropertyName] = value; }
        }

        public bool UseGraphite
        {
            get { return this.Properties.Contains(GraphitePropertyName); }
        }

        /// <summary>
        /// Gets or sets the Counters.
        /// </summary>
        [ConfigurationPropertyAttribute(StatsDPropertyName)]
        public StatsDElement StatsD
        {
            get { return (StatsDElement)this[StatsDPropertyName]; }
            set { base[StatsDPropertyName] = value; }
        }

        public bool UseStatsD
        {
            get { return this.Properties.Contains(StatsDPropertyName); }
        }
    }
}
