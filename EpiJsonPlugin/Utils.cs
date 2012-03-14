﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.Web.PropertyControls;
using System.IO;
using System.Web.UI;

namespace EpiJsonPlugin
{
    public static class Utils
    {
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
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }

        //http://tedgustaf.com/en/blog/2009/9/parse-an-episerver-xhtml-property-with-dynamic-content/
        public static string ParseHtmlProperty(PropertyData propertydata)
        {
            // Create a Property control which will parse the XHTML value for us
            var ctrl = new PropertyLongStringControl();

            // Set the PropertyData to the property we want to parse
            ctrl.PropertyData = propertydata;

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