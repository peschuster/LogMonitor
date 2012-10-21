using System.Configuration;

namespace LogMonitor.Configuration
{
    [ConfigurationCollectionAttribute(typeof(ParserElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ParserElementCollection : GenericElementCollection<ParserElement>
    {
        /// <summary>
        /// Gets the element key for the specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="ConfigurationElement"/>.
        /// </returns>        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParserElement)element).ScriptPath;
        }
    }
}
