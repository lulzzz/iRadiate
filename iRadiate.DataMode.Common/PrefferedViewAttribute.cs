using System;


namespace iRadiate.DataModel
{
    [System.AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class PreferredViewAttribute : System.Attribute
    {
        public string ViewName;
        public string AssemblyName;

        public PreferredViewAttribute(string viewName, string assemblyName)
        {
            this.ViewName = viewName;
            this.AssemblyName = assemblyName;
        }
        
    }
}
