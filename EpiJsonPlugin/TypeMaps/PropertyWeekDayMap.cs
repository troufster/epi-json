using EPiServer.Core;
using EPiServer.SpecializedProperties;
using EPiServer.WebParts.Core;

namespace EpiJsonPlugin.TypeMaps
{

    [TypeMap(PropertyType = typeof(PropertyWeekDay))]
    public class PropertyWeekDayMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return propertyData.ToString();
        }
    }
    
}
