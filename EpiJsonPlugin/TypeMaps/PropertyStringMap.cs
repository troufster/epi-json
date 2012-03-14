using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyString))]
    public class PropertyStringMap : ITypeMapTemplate
    {
        public string Map(EPiServer.Core.PageData pageData, EPiServer.Core.PropertyData propertyData)
        {
            throw new NotImplementedException();
        }
    }
}
