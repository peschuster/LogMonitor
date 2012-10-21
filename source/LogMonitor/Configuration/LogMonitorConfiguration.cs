using System.Configuration;

namespace LogMonitor.Configuration
{
    public class LogMonitorConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The XML name of the LogMonitorConfiguration Configuration Section.
        /// </summary>        
        internal const string LogMonitorConfigurationSectionName = "logMonitor";

        /// <summary>
        /// The XML name of the <see cref="Watch"/> property.
        /// </summary>        
        internal const string WatchPropertyName = "watch";

        /// <summary>
        /// The XML name of the <see cref="Parser"/> property.
        /// </summary>        
        internal const string ParserPropertyName = "parser";

        /// <summary>
        /// The XML name of the <see cref="Output"/> property.
        /// </summary>        
        internal const string OutputPropertyName = "output";

        /// <summary>
        /// The XML name of the <see cref="Xmlns"/> property.
        /// </summary>        
        internal const string XmlnsPropertyName = "xmlns";

        /// <summary>
        /// Gets the LogMonitorConfiguration instance.
        /// </summary>        
        public static LogMonitorConfiguration Instance
        {
            get { return (LogMonitorConfiguration)ConfigurationManager.GetSection(LogMonitorConfigurationSectionName); }
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
        /// Gets or sets the Watch elements.
        /// </summary>
        [ConfigurationPropertyAttribute(WatchPropertyName)]
        public WatchElementCollection Watch
        {
            get { return (WatchElementCollection)this[WatchPropertyName]; }
            set { base[WatchPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Parser elements.
        /// </summary>
        [ConfigurationPropertyAttribute(ParserPropertyName)]
        public ParserElementCollection Parser
        {
            get { return (ParserElementCollection)this[ParserPropertyName]; }
            set { base[ParserPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Output elements.
        /// </summary>
        [ConfigurationPropertyAttribute(OutputPropertyName)]
        public OutputElementCollection Output
        {
            get { return (OutputElementCollection)this[OutputPropertyName]; }
            set { base[OutputPropertyName] = value; }
        }
    }
}
