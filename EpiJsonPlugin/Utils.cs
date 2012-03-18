using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Web.PropertyControls;
using System.IO;
using System.Web.UI;
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

        
        public static double UnixTicks(DateTime dt)
        {
            var d1 = new DateTime(1970, 1, 1);
            var d2 = dt.ToUniversalTime();
            var ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }

        //http://tedgustaf.com/en/blog/2009/9/parse-an-episerver-xhtml-property-with-dynamic-content/
        public static string ParseHtmlProperty(PropertyData propertydata)
        {
            // Create a Property control which will parse the XHTML value for us
            var ctrl = new PropertyLongStringControl {PropertyData = propertydata};

            // Set the PropertyData to the property we want to parse

            // Initialize the Property control
            ctrl.SetupControl();

            // Create a string writer...
            var sw = new StringWriter();

            // ...and an HtmlTextWriter using that string writer
            var htw = new HtmlTextWriter(sw);

            // Render the Property control to get the markup
            ctrl.RenderControl(htw);

            // Return the parsed markup
            return sw.ToString();
        }
    }
}
