using System.IO;
using System.Web.UI;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using EPiServer.Web.PropertyControls;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyXhtmlString))]
    public class PropertyXhtmlStringMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            //http://tedgustaf.com/en/blog/2009/9/parse-an-episerver-xhtml-property-with-dynamic-content/    

            // Create a Property control which will parse the XHTML value for us
            var ctrl = new PropertyLongStringControl { PropertyData = propertyData };

            // Set the PropertyData to the property we want to parse
            // Initialize the Property control
            ctrl.SetupControl();

            // Create a string writer...
            var sw = new StringWriter();

            // ...and an HtmlTextWriter using that string writer
            var htw = new HtmlTextWriter(sw);

            // Render the Property control to get the markup
            ctrl.RenderControl(htw);

            return string.Format("\"{0}\"", Utils.EscapeStringForJs(sw.ToString()));            
        }
    }
}
