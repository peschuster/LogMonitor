using System.Configuration;

namespace LogMonitor.Configuration
{
    [ConfigurationCollectionAttribute(typeof(OutputElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class OutputElementCollection : GenericElementCollection<OutputElement>
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
            return ((OutputElement)element).PathPattern;
        }
    }
}
