using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Coderr.Client.Contracts
{
    /// <summary>
    ///     DTO for a context collection
    /// </summary>
    [DataContract]
    public class ContextCollectionDTO
    {
        private string _name;

        private ContextCollectionDTO()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextCollectionDTO" /> class.
        /// </summary>
        /// <param name="name">Collection name (will be shown in the web site).</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public ContextCollectionDTO(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            Properties = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextCollectionDTO" /> class.
        /// </summary>
        /// <param name="name">Collection name (will be shown in the web site).</param>
        /// <param name="properties">All Properties in this collection.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     name
        ///     or
        ///     Properties
        /// </exception>
        public ContextCollectionDTO(string name, IDictionary<string, string> properties)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (properties == null) throw new ArgumentNullException("properties");
            Name = name;
            Properties = properties;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextCollectionDTO" /> class.
        /// </summary>
        /// <param name="name">Collection name (will be shown in the web site).</param>
        /// <param name="properties">All Properties in this collection.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     name
        ///     or
        ///     Properties
        /// </exception>
        public ContextCollectionDTO(string name, Dictionary<string, string> properties)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (properties == null) throw new ArgumentNullException("properties");
            Name = name;
            Properties = properties;
        }


        /// <summary>
        ///     Gets name of this collection (shown for the user so that he/she can identify this collection)
        /// </summary>
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _name = value;
            }
        }

        /// <summary>
        ///     All Properties in the collection
        /// </summary>
        [DataMember]
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name + " [" + string.Join("\r\n\t",
                       Properties.Select(x => x.Key + "=" + x.Value)) + "]\r\n";
        }
    }
}