using System;


namespace iRadiate.DataModel
{

    /// <summary>
    /// The system will audit all changes to properties marked with auditable
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]   
    public class AuditableAttribute : System.Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AuditableAttribute()
        {
            //Do we need to put anything here?
        }
    }

    /// <summary>
    /// This property can be used to filter or retrieve 
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QueryableAttribute : System.Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QueryableAttribute()
        {

        }

        public QueryableAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}
