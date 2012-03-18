using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    
    [TypeMap(PropertyType = typeof(PropertyFloatNumber))]
    public class PropertyFloatMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return propertyData.ToString();
        }
    }
    
}
