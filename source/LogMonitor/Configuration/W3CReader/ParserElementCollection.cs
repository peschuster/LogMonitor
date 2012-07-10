using System.Configuration;

namespace LogMonitor.Configuration.W3CReader
{
    [ConfigurationCollectionAttribute(typeof(ParserElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ParserElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets the type of the <see cref="ConfigurationElementCollection"/>.
        /// </summary>
        /// <returns>The <see cref="ConfigurationElementCollectionType"/> of this collection.</returns>        
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        /// <summary>
        /// Gets the element key for the specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="ConfigurationElement"/>.
        /// </returns>        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParserElement)element).Name;
        }

        /// <summary>
        /// Creates a new <see cref="ParserElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="ParserElement"/>.
        /// </returns>        
        protected override ConfigurationElement CreateNewElement()
        {
            return new ParserElement();
        }

        /// <summary>
        /// Gets the <see cref="ParserElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="ParserElement"/> to retrieve.</param>        
        public ParserElement this[int index]
        {
            get { return (ParserElement)this.BaseGet(index); }
        }

        /// <summary>
        /// Gets the <see cref="ParserElement"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="ParserElement"/> to retrieve.</param>        
        public ParserElement this[object name]
        {
            get { return (ParserElement)this.BaseGet(name); }
        }

        /// <summary>
        /// Gets the <see cref="ParserElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="ParserElement"/> to retrieve.</param>        
        public ParserElement GetItemAt(int index)
        {
            return (ParserElement)base.BaseGet(index);
        }

        /// <summary>
        /// Gets the <see cref="ParserElement"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="ParserElement"/> to retrieve.</param>        
        public ParserElement GetItemByKey(string name)
        {
            return (ParserElement)base.BaseGet((object)(name));
        }
    }
}
