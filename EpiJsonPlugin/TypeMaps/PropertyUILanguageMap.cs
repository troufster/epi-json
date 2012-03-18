using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{
    
    [TypeMap(PropertyType = typeof(PropertyUILanguage))]
    public class PropertyUiLanguageMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }
    
}
