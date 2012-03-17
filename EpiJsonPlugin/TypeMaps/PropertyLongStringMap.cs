using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType=typeof(PropertyLongString))]
    public class PropertyLongStringMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", Utils.EscapeStringForJs(propertyData.ToString()));
        }
    }
}
