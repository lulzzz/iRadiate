using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Common
{
    /// <summary>
    /// Represents an alteration to an item in the datastore
    /// </summary>
    /// <remarks>
    /// Using reflection it should be possible to recreate the object in its previous state.
    /// </remarks>
    public interface IAlteration
    {
        
        
        
        /// <summary>
        /// The name of the property on the object that was altered
        /// </summary>
        String PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the old value of the property.
        /// </summary>
        string OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value of the property
        /// </summary>
        string NewValue { get; set; }

        int ItemIDNumber { get; set; }


    }
}
