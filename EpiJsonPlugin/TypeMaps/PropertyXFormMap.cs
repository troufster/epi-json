using System;
using EPiServer.Core;
using EPiServer.XForms;
using EpiJsonPlugin;

namespace EpiJsonPlugin.TypeMaps
{

    [TypeMap(PropertyType = typeof(PropertyXForm))]
    public class PropertyXFormMap : ITypeMapTemplate
    {
        public string Map(PageData pageData, PropertyData propertyData)
        {
            return string.Format("\"{0}\"", propertyData);
        }
    }
    
}
