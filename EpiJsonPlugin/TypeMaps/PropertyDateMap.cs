using System;
using System.Globalization;
using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    [TypeMap(PropertyType = typeof(PropertyDate))]
    public class PropertyDateMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            var propertyDate = propertyData as PropertyDate;
            if (propertyDate != null)
            {
                var d = propertyDate.Date;
                return d != default(DateTime) ? Utils.UnixTicks(d).ToString(CultureInfo.InvariantCulture) : (-1).ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }
    }
}
