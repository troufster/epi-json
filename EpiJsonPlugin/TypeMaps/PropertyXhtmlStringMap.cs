using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyXhtmlString))]
    public class PropertyXhtmlStringMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {         
            var propstr = Utils.ParseHtmlProperty(propertyData);          

            return string.Format("\"{0}\"", Utils.EscapeStringForJs(propstr));            
        }
    }
}
