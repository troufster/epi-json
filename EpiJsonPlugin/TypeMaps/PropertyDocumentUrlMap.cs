using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{

    [TypeMap(PropertyType = typeof(PropertyDocumentUrl))]
    public class PropertyDocumentUrlMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }

    [TypeMap(PropertyType = typeof(PropertyUrl))]
    public class PropertyUrlMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }

    [TypeMap(PropertyType = typeof(PropertyImageUrl))]
    public class PropertyImageUrllMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }
    
}
