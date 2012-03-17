using System;
using System.Linq;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyLinkCollection))]
    public class PropertyLinkCollectionMap : ITypeMapTemplate
    {
        public string Map(EPiServer.Core.PageData pageData, EPiServer.Core.PropertyData propertyData)
        {
            var links = propertyData as PropertyLinkCollection;
            if (links != null)
                return string.Format("[ {0} ]",
                                     String.Join(",", (string[]) links.Select(
                                         l => string.Format("{{ \"href\":\"{0}\", \"text\":\"{1}\", \"target\":\"{2}\", \"title\":\"{3}\"  }}", l.Href, l.Text, l.Target, l.Title
                                                  ))));

            return "[]";
        }
    }
}
