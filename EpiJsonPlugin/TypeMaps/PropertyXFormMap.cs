using EPiServer.Core;
using EPiServer.SpecializedProperties;
using EPiServer.WebParts.Core;

namespace EpiJsonPlugin.TypeMaps
{

    [TypeMap(PropertyType = typeof(PropertyXForm))]
    public class PropertyXFormMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }
    
}
