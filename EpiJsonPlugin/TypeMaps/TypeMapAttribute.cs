using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpiJsonPlugin.TypeMaps
{
    [AttributeUsage(AttributeTargets.Class)]
    class TypeMapAttribute : System.Attribute 
    {
        public Type PropertyType             
        {
            get;
            set;
        }
    }
}
