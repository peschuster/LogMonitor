using System.Configuration;

namespace LogMonitor.Configuration
{
    public class ParserElement : ConfigurationElement
    {
        /// <summary>
        /// The XML name of the <see cref="ScriptPath"/> property.
        /// </summary>        
        internal const string ScriptPathPropertyName = "scriptPath";

        /// <summary>
        /// The XML name of the <see cref="Pattern"/> property.
        /// </summary>
        internal const string PatternPropertyName = "pattern";

        /// <summary>
        /// Gets or sets the ScriptPath.
        /// </summary>        
        [ConfigurationPropertyAttribute(ScriptPathPropertyName, IsRequired = true, IsKey = true)]
        public string ScriptPath
        {
            get { return (string)base[ScriptPathPropertyName]; }
            set { base[ScriptPathPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Pattern.
        /// </summary>
        [ConfigurationPropertyAttribute(PatternPropertyName, IsRequired = true)]
        public string Pattern
        {
            get { return (string)base[PatternPropertyName]; }
            set { base[PatternPropertyName] = value; }
        }
    }
}