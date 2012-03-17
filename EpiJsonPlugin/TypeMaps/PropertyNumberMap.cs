using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyNumber))]
    public class PropertyNumberMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return propertyData.ToString();
        }
    }
}
