using System.Configuration;

namespace LogMonitor.Configuration
{
    public class OutputElement : ConfigurationElement
    {
        /// <summary>
        /// The XML name of the <see cref="PathPattern"/> property.
        /// </summary>        
        internal const string PathPatternPropertyName = "pathPattern";

        /// <summary>
        /// The XML name of the <see cref="Type"/> property.
        /// </summary>
        internal const string TypePropertyName = "type";

        /// <summary>
        /// The XML name of the <see cref="Target"/> property.
        /// </summary>
        internal const string TargetPropertyName = "target";

        /// <summary>
        /// The XML name of the <see cref="MetricsPrefix"/> property.
        /// </summary>
        internal const string MetricsPrefixPropertyName = "metricsPrefix";

        /// <summary>
        /// Gets or sets the PathPattern.
        /// </summary>        
        [ConfigurationPropertyAttribute(PathPatternPropertyName, IsRequired = true, IsKey = true)]
        public string PathPattern
        {
            get { return (string)base[PathPatternPropertyName]; }
            set { base[PathPatternPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        [ConfigurationPropertyAttribute(TypePropertyName, IsRequired = true)]
        public string Type
        {
            get { return (string)base[TypePropertyName]; }
            set { base[TypePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Target.
        /// </summary>
        [ConfigurationPropertyAttribute(TargetPropertyName, IsRequired = true)]
        public string Target
        {
            get { return (string)base[TargetPropertyName]; }
            set { base[TargetPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the MetricsPrefix.
        /// </summary>
        [ConfigurationPropertyAttribute(MetricsPrefixPropertyName, IsRequired = true)]
        public string MetricsPrefix
        {
            get { return (string)base[MetricsPrefixPropertyName]; }
            set { base[MetricsPrefixPropertyName] = value; }
        }
    }
}
