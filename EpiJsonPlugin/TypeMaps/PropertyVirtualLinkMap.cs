using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{

    [TypeMap(PropertyType = typeof(PropertyVirtualLink))]
    public class PropertyVirtualLinkMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }
    
}
