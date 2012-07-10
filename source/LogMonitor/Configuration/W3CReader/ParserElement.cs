using System.Configuration;

namespace LogMonitor.Configuration.W3CReader
{
    public class ParserElement : ConfigurationElement
    {
        /// <summary>
        /// The XML name of the <see cref="Name"/> property.
        /// </summary>        
        internal const string NamePropertyName = "name";

        /// <summary>
        /// The XML name of the <see cref="Key"/> property.
        /// </summary>
        internal const string KeyPropertyName = "key";

        /// <summary>
        /// The XML name of the <see cref="FileMatch"/> property.
        /// </summary>        
        internal const string FileMatchPropertyName = "fileMatch";

        /// <summary>
        /// The XML name of the <see cref="Target"/> property.
        /// </summary>        
        internal const string TargetPropertyName = "target";

        /// <summary>
        /// The XML name of the <see cref="Pattern"/> property.
        /// </summary>        
        internal const string PatternPropertyName = "pattern";

        /// <summary>
        /// The XML name of the <see cref="KeyPatternTarget"/> property.
        /// </summary>        
        internal const string KeyPatternTargetPropertyName = "keyPatternTarget";

        /// <summary>
        /// The XML name of the <see cref="KeyPattern"/> property.
        /// </summary>        
        internal const string KeyPatternPropertyName = "keyPattern";

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>        
        [ConfigurationPropertyAttribute(NamePropertyName, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Key.
        /// </summary>
        [ConfigurationPropertyAttribute(KeyPropertyName, IsRequired = true)]
        public string Key
        {
            get { return (string)base[KeyPropertyName]; }
            set { base[KeyPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the FileMatch.
        /// </summary>
        [ConfigurationPropertyAttribute(FileMatchPropertyName, IsRequired = true)]
        public string FileMatch
        {
            get { return (string)base[FileMatchPropertyName]; }
            set { base[FileMatchPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Target.
        /// </summary>
        [ConfigurationPropertyAttribute(TargetPropertyName, IsRequired = false)]
        public string Target
        {
            get { return (string)base[TargetPropertyName]; }
            set { base[TargetPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the Pattern.
        /// </summary>
        [ConfigurationPropertyAttribute(PatternPropertyName, IsRequired = false)]
        public string Pattern
        {
            get { return (string)base[PatternPropertyName]; }
            set { base[PatternPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the key pattern target.
        /// </summary>
        [ConfigurationPropertyAttribute(KeyPatternTargetPropertyName, IsRequired = false)]
        public string KeyPatternTarget
        {
            get { return (string)base[KeyPatternTargetPropertyName]; }
            set { base[KeyPatternTargetPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the key pattern.
        /// </summary>
        [ConfigurationPropertyAttribute(KeyPatternPropertyName, IsRequired = false)]
        public string KeyPattern
        {
            get { return (string)base[KeyPatternPropertyName]; }
            set { base[KeyPatternPropertyName] = value; }
        }
    }
}
