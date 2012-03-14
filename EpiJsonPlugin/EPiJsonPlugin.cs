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


namespace EPiServer.Plugins
{
    [PagePlugIn(DisplayName = "JSON Exporter", Description = "Exports pages as JSON")]
    public class EPiJsonPlugin
    {

        private static Dictionary<Type, int> _typeDict = new Dictionary<Type, int>
        {
             {typeof(PropertyBoolean),0},
             {typeof(PropertyLinkCollection),1},
             {typeof(PropertyXhtmlString),2}
        };


        public static void Initialize(int bitflags)
        {
            EPiServer.PageBase.PageSetup += PageBase_BaseSetup;
        }

        static void PageBase_BaseSetup(EPiServer.PageBase sender, EPiServer.PageSetupEventArgs e)
        {
            sender.Load += sender_Load;
        }

        static void FilterPageDataProperties(PageData pd, Dictionary<string, string> dict)
        {

            foreach (PropertyData prop in pd.Property)
            {

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
                    case 2:  //PropertyXhtmlString
                        propval = string.Format("\"{0}\"", HttpUtility.HtmlEncode(prop.ToString()));
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

                //Todo: switch options properly

                //Return current page as JSON
                var dict = new Dictionary<string, string>();

                FilterPageDataProperties(pb.CurrentPage, dict);

                //Convert to json
                var json = DictToJson(dict);

                pb.Response.Headers.Add("Content-type", "application/json");
                pb.Response.Write(json);
                pb.Response.End();

            }
        }

    }
}