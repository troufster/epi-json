using System;

namespace EpiJsonPlugin.TypeMaps
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeMapAttribute : Attribute 
    {
        public Type PropertyType             
        {
            get;
            set;
        }
    }
}
