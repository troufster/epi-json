//Work continued on Allan's blogpost from 2009
//http://labs.episerver.com/en/Blogs/Allan/Dates/2009/3/Output-PageData-as-JSON/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.PlugIn;
using EPiServer.Core;
using System.Text;
using EPiServer.Filters;
using EPiServer;
using EPiServer.SpecializedProperties;
using EPiServer.Web.PropertyControls;
using System.IO;
using System.Web.UI;
using EpiJsonPlugin;
using System.Reflection;
using EpiJsonPlugin.TypeMaps;


namespace EPiServer.Plugins
{
    [PagePlugIn(DisplayName = "JSON Exporter", Description = "Exports pages as JSON")]
    public class EPiJsonPlugin
    {
        private static IEnumerable<Type> _typeMaps;

        private static void LoadTypeMaps() {
            var assemblies = Utils.GetLoadedAssemblies();
            var types = new List<Type>();

            foreach(var assembly in assemblies) {
                var typesInAsm = Utils.GetTypesWithTypeMapAttribute(assembly);
                types.AddRange(typesInAsm);
            }

            _typeMaps = types;

            //Activate types
            foreach (var type in types) {
                var instance = Activator.CreateInstance(type);

                var attribs = type.GetCustomAttributes(typeof(TypeMapAttribute), true);

                //Todo: also overwrite.
                _typeMapDict.Add((attribs[0] as TypeMapAttribute).PropertyType, instance as ITypeMapTemplate);
            }
        }

        private static Dictionary<Type, ITypeMapTemplate> _typeMapDict = new Dictionary<Type, ITypeMapTemplate>();

        private static Dictionary<Type, int> _typeDict = new Dictionary<Type, int>
        {
             {typeof(PropertyBoolean),0},
             {typeof(PropertyLinkCollection),1},
             {typeof(PropertyXhtmlString),2},
             {typeof(PropertyNumber),3},
             {typeof(PropertyPageReference),4},
             {typeof(PropertyString),5},
             {typeof(PropertyLongString),6},
             {typeof(PropertyDate),7}

        };

        private static string[] _privateProps = { };

        public static void Initialize(int bitflags)
        {
            EPiServer.PageBase.PageSetup += PageBase_BaseSetup;
            LoadTypeMaps();
        }

        static void PageBase_BaseSetup(EPiServer.PageBase sender, EPiServer.PageSetupEventArgs e)
        {
            sender.Load += sender_Load;
        }

        static void FilterPageDataProperties(PageData pd, Dictionary<string, string> dict, string[] onlyProps=null)
        {

            foreach (PropertyData prop in pd.Property)
            {
                //Filter properties if told to
                if (onlyProps != null && !onlyProps.Contains(prop.Name)) {
                    continue;
                }


                string propval = string.Empty;
                int typeval = -1;


                if (!_typeDict.TryGetValue(prop.GetType(), out typeval))
                    continue;

                switch (typeval)
                {
                    case 0:  //PropertyBoolean

                        propval = string.Format("\"{0}\"", (prop.Value != null).ToString());
                        break;

                    case 1:  //PropertyLinkCollection
                        var links = prop as PropertyLinkCollection;
                        propval = string.Format("[ {0} ]",
                            string.Join(",", links.Select(
                                l => string.Format("{{ \"href\":\"{0}\", \"text\":\"{1}\", \"target\":\"{2}\", \"title\":\"{3}\"  }}", l.Href, l.Text, l.Target, l.Title
                            ))));
                        break;

                    case 6:  //PropertyLongString
                    case 5:  //PropertyString
                    case 2:  //PropertyXhtmlString

                        var propstr = prop.ToString();
                        //Render dynamic content
                        if (typeval == 2)
                        {
                            propstr = Utils.ParseHtmlProperty(prop as PropertyData);
                        }

                        propval = string.Format("\"{0}\"", Utils.EscapeStringForJs(propstr));
                        break;

                    case 3: //PropertyNumber
                        propval = prop.ToString();
                        break;

                    case 4: //PropertyPagereference
                        propval = prop.ToString();
                        break;

                    case 7: //PropertyDate
                        var d = (prop as PropertyDate).Date;
                        propval = d != default(DateTime) ? Utils.UnixTicks((prop as PropertyDate).Date).ToString() : (-1).ToString();
                        break;
                    default:

                        break;
                }

                if (!string.IsNullOrEmpty(propval))
                {
                    dict.Add(prop.Name, propval);
                }
            }
        }

        static string DictToJson(Dictionary<string, string> dict)
        {
            var contents = string.Join(",", dict.Select(d => string.Format("\"{0}\" : {1}", d.Key, d.Value)));
            return String.Format("{{ {0} }}", contents);
        }

        static void sender_Load(object sender, EventArgs e)
        {
            var pb = sender as PageBase;

            //Check if json was requested
            if (!string.IsNullOrEmpty(pb.Request["json"]))
            {
                //Parse option
                var option = pb.Request["json"].ToLower();
                string json = string.Empty;

                switch (option) { 
                    case "current": //Current page only                     
                        var dict = new Dictionary<string, string>();

                        FilterPageDataProperties(pb.CurrentPage, dict);

                        //Convert to json
                        json = DictToJson(dict);
                        break;

                    case "children": //Page children
                        var fa=new FilterAccess(EPiServer.Security.AccessLevel.Read);
                        var pdc = pb.GetChildren(pb.CurrentPageLink);
                        var pages = new List<string>();

                        fa.Filter(pdc);

                        foreach (var pd in pdc)
                        {
                            dict = new Dictionary<string, string>();
                            FilterPageDataProperties(pd, dict);
                          
                            pages.Add(DictToJson(dict));
                        }

                        json = pages.Count > 0 ? string.Format("[ {0} ]",string.Join(",", pages)) : string.Empty;
                        break;
                    case "childrenids": //Page children ids only
                         fa=new FilterAccess(EPiServer.Security.AccessLevel.Read);
                         pdc = pb.GetChildren(pb.CurrentPageLink);
                         pages = new List<string>();

                        fa.Filter(pdc);

                        foreach (var pd in pdc)
                        {
                            dict = new Dictionary<string, string>();
                            FilterPageDataProperties(pd, dict, new [] {"PageLink"});
                          
                            pages.Add(DictToJson(dict));
                        }

                        json = pages.Count > 0 ? string.Format("[ {0} ]",string.Join(",", pages)) : string.Empty;
                        break;
                
                }

                pb.Response.ContentType = "application/json";
                pb.Response.Write(json);
                pb.Response.End();

            }
        }
    }

}