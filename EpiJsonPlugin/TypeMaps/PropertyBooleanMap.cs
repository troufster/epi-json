using System.Globalization;
using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyBoolean))]
    public class PropertyBooleanMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", (propertyData.Value != null).ToString(CultureInfo.InvariantCulture));
        }
    }
}
