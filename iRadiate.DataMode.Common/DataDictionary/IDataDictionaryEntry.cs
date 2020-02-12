using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.DataDictionary
{
    /// <summary>
    /// Interface for an entry in the DataDictionary that defines all the terms that have been invented by the user
    /// </summary>
    public interface IDataDictionaryEntry
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        /// <remarks>
        /// Shuould be unique and readable
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// The code of the variable
        /// </summary>
        /// <remarks>
        /// Should be unique and brief and using CamelCase
        /// </remarks>
        string Code { get; set; }

        /// <summary>
        /// A description of the variable
        /// </summary>
        /// <remarks>
        /// Can be as long as needed and gives the user a clear understanding of what the variable is for
        /// </remarks>
        string Description { get; set; }

        /// <summary>
        /// The namespace to which this entry belongs
        /// </summary>
        /// <remarks>
        /// The datadictionary is grouped into namespaces for the sake of uniqueness
        /// </remarks>
        DataDictionaryNamespace Namespace { get; set; }

        /// <summary>
        /// The name of the entry preceded by the fullname of the name space
        /// </summary>
        string FullName { get;  }

        /// <summary>
        /// Gets or sets whether the entry is available for use
        /// </summary>
        bool Active { get; set; }


    }
}
