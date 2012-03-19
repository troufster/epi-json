using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EpiJsonPlugin
{
    public static class Utils
    {

        public static IEnumerable<Assembly> GetLoadedAssemblies() {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return loadedAssemblies;
        }

        public static IEnumerable<Type> GetTypesWithAttribute<T>(Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(T), true).Length > 0);
        }

        //  \b  Backspace (ascii code 08)
        //  \f  Form feed (ascii code 0C)
        //  \n  New line
        //  \r  Carriage return
        //  \t  Tab
        //  \v  Vertical tab
        //  \'  Apostrophe or single quote
        //  \"  Double quote
        //  \\  Backslash caracter
        public static string EscapeStringForJs(string input)
        {
            input = input.Replace("\\", @"\\");
            input = input.Replace("\b", @"\b");
            input = input.Replace("\f", @"\f");
            input = input.Replace("\n", @"\n");
            input = input.Replace("\r", @"\r");
            input = input.Replace("\t", @"\t");
            input = input.Replace("\v", @"\v");
            input = input.Replace("\"", @"\""");
            input = input.Replace("\'", @"\'");
            return input;
        }
    }
}
