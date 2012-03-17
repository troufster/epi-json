using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    public interface ITypeMapTemplate
    {
         string Map(PageData pageData, PropertyData propertyData);
    }
}
