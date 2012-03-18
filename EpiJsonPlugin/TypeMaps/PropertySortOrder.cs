using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyFileSortOrder))]
    public class PropertyFileSortOrderMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return propertyData.ToString();
        }
    }

    [TypeMap(PropertyType = typeof(PropertySortOrder))]
    public class PropertySortOrderMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return propertyData.ToString();
        }
    }
    
}
