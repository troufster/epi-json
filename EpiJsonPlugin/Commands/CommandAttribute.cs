using System;

namespace EpiJsonPlugin.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public String CommandName
        {
            get;
            set;
        }
    }
}
