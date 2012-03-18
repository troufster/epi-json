using EPiServer.Core;
using EPiServer.SpecializedProperties;
using System.Linq;

namespace EpiJsonPlugin.TypeMaps
{
    
    [TypeMap(PropertyType = typeof(PropertyLanguageBranch))]
    public class PropertyLanguageBranchListMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }
    
}
