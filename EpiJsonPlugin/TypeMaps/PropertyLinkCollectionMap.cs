using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyLinkCollection))]
    public class PropertyLinkCollectionMap : ITypeMapTemplate
    {
        public string Map(EPiServer.Core.PageData pageData, EPiServer.Core.PropertyData propertyData)
        {
            var links = propertyData as PropertyLinkCollection;
            return string.Format("[ {0} ]",
                string.Join(",", links.Select(
                    l => string.Format("{{ \"href\":\"{0}\", \"text\":\"{1}\", \"target\":\"{2}\", \"title\":\"{3}\"  }}", l.Href, l.Text, l.Target, l.Title
                ))));
            
        }
    }
}
