using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    
    [TypeMap(PropertyType = typeof(PropertyCategory))]
    public class PropertyCategoryMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("[{0}]",propertyData);
        }
    }
    
}
