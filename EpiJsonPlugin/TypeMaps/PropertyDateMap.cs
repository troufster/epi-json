using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyDate))]
    public class PropertyDateMap : ITypeMapTemplate
    {
        public string Map(EPiServer.Core.PageData pageData, EPiServer.Core.PropertyData propertyData)
        {
           var d = (propertyData as PropertyDate).Date;
           return d != default(DateTime) ? Utils.UnixTicks(d).ToString() : (-1).ToString();           
        }
    }
}
