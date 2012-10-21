using System.Configuration;

namespace LogMonitor.Configuration
{
    public abstract class GenericElementCollection<T> : ConfigurationElementCollection
        where T : ConfigurationElement, new()
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
        /// Creates a new <see cref="WatchElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="WatchElement"/>.
        /// </returns>        
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="ConfigurationElement"/> to retrieve.</param>        
        public T this[int index]
        {
            get { return (T)this.BaseGet(index); }
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationElement"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="ConfigurationElement"/> to retrieve.</param>        
        public T this[object name]
        {
            get { return (T)this.BaseGet(name); }
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="ConfigurationElement"/> to retrieve.</param>        
        public T GetItemAt(int index)
        {
            return (T)base.BaseGet(index);
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationElement"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="ConfigurationElement"/> to retrieve.</param>        
        public T GetItemByKey(string name)
        {
            return (T)base.BaseGet((object)(name));
        }
    }
}
