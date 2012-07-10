using System.Configuration;
using LogMonitor.Configuration.W3CReader;

namespace LogMonitor.Configuration
{
    public class W3CReaderConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The XML name of the W3CReadersConfiguration Configuration Section.
        /// </summary>        
        internal const string W3CReadersConfigurationSectionName = "w3cReaders";

        /// <summary>
        /// The XML name of the <see cref="Counters"/> property.
        /// </summary>        
        internal const string CountersPropertyName = "counters";

        /// <summary>
        /// The XML name of the <see cref="Timings"/> property.
        /// </summary>        
        internal const string TimingsPropertyName = "timings";

        /// <summary>
        /// The XML name of the <see cref="Xmlns"/> property.
        /// </summary>        
        internal const string XmlnsPropertyName = "xmlns";

        /// <summary>
        /// Gets the W3CReadersConfiguration instance.
        /// </summary>        
        public static W3CReaderConfiguration Instance
        {
            get { return (W3CReaderConfiguration)ConfigurationManager.GetSection(W3CReadersConfigurationSectionName); }
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
        [ConfigurationPropertyAttribute(CountersPropertyName)]
        public ParserElementCollection Counters
        {
            get { return (ParserElementCollection)this[CountersPropertyName]; }
            set { base[CountersPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Timings.
        /// </summary>
        [ConfigurationPropertyAttribute(TimingsPropertyName)]
        public ParserElementCollection Timings
        {
            get { return (ParserElementCollection)this[TimingsPropertyName]; }
            set { base[TimingsPropertyName] = value; }
        }
    }
}
