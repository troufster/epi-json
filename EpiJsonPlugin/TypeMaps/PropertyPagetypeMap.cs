using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyPageType))]
    public class PropertyPageTypeMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return propertyData.Value == null ? "-1" : propertyData.ToString();
        }
    }
}
