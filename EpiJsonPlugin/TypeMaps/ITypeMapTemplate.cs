using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;

namespace EpiJsonPlugin.TypeMaps
{
    public interface ITypeMapTemplate
    {
         string Map(PageData pageData, PropertyData propertyData);
    }
}
