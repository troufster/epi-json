using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyXhtmlString))]
    public class PropertyXhtmlStringMap : ITypeMapTemplate
    {
        public string Map(EPiServer.Core.PageData pageData, EPiServer.Core.PropertyData propertyData)
        {         
            var propstr = Utils.ParseHtmlProperty(propertyData as PropertyData);          

            return string.Format("\"{0}\"", Utils.EscapeStringForJs(propstr));            
        }
    }
}
