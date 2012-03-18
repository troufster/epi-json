using EPiServer.Core;
using EPiServer.SpecializedProperties;
using System.Linq;

namespace EpiJsonPlugin.TypeMaps
{
    
    [TypeMap(PropertyType = typeof(PropertySelector))]
    public class PropertyListMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return propertyData.Value == null ? "[]" : string.Format("[{0}]", string.Join(",", propertyData.Value.ToString().Split(',').Select(o => string.Format("\"{0}\"", o))));
        }
    }
    
}
