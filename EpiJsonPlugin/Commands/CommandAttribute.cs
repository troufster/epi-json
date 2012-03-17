using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpiJsonPlugin.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : System.Attribute
    {
        public String CommandName
        {
            get;
            set;
        }
    }
}
