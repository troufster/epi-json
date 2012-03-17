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
using EpiJsonPlugin.Commands;


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
                var typesInAsm = Utils.GetTypesWithAttribute<TypeMapAttribute>(assembly);
                types.AddRange(typesInAsm);
            }

            _typeMaps = types;

            //Activate types
            foreach (var type in types) {
                var instance = Activator.CreateInstance(type);

                var attribs = type.GetCustomAttributes(typeof(TypeMapAttribute), true);
                var propertyType = (attribs[0] as TypeMapAttribute).PropertyType;
               
                //Overwrite type map redeclarations
                if (_typeMapDict.ContainsKey(propertyType)) {
                    _typeMapDict.Remove(propertyType);
                }
                
                _typeMapDict.Add(propertyType, instance as ITypeMapTemplate);
            }
        }

        private static void LoadCommands() {
            var assemblies = Utils.GetLoadedAssemblies();
            var types = new List<Type>();

            foreach (var assembly in assemblies) {
                var typesInAsm = Utils.GetTypesWithAttribute<CommandAttribute>(assembly);
                types.AddRange(typesInAsm);
            }

            //Activate types
            foreach (var type in types) {
                var instance = Activator.CreateInstance(type);

                var attribs = type.GetCustomAttributes(typeof(CommandAttribute), true);
                var cmdName = (attribs[0] as CommandAttribute).CommandName;
                
                //Overwrite redeclaration of cmdname
                if(_commandDict.ContainsKey(cmdName)) {
                    _commandDict.Remove(cmdName);
                }
                _commandDict.Add(cmdName, instance as ICommandTemplate);
            }
        }

        private static Dictionary<string, ICommandTemplate> _commandDict = new Dictionary<string, ICommandTemplate>(); 
        private static Dictionary<Type, ITypeMapTemplate> _typeMapDict = new Dictionary<Type, ITypeMapTemplate>();

        private static string[] _privateProps = { };

        public static void Initialize(int bitflags)
        {
            EPiServer.PageBase.PageSetup += PageBase_BaseSetup;
            LoadTypeMaps();
            LoadCommands();
        }

        static void PageBase_BaseSetup(EPiServer.PageBase sender, EPiServer.PageSetupEventArgs e)
        {
            sender.Load += sender_Load;
        }



        static void FilterPageDataProperties(PageData pd, Dictionary<string, string> dict, string[] onlyProps=null)
        {

            foreach (PropertyData prop in pd.Property)
            {
                //Filter out properties if told to
                if (onlyProps != null && !onlyProps.Contains(prop.Name)) {
                    continue;
                }

                ITypeMapTemplate mapTemplate;

                if (!_typeMapDict.TryGetValue(prop.GetType(), out mapTemplate)) continue;

                string propval = string.Empty;
                propval = mapTemplate.Map(pd, prop);
            
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

            //Todo: allow definition of command listeners by plugins
            //Check if json was requested
            if (!string.IsNullOrEmpty(pb.Request["json"]))
            {
                //Parse option
                var command = pb.Request["json"].ToLower();
                string json = string.Empty;

                ICommandTemplate cmd;

                if (!_commandDict.TryGetValue(command, out cmd)) pb.Response.End();

                var commandSelection = cmd.ExecuteCommand(pb);
                var filter = cmd.GetPropertyFilter();
                var pages = new List<string>();

                foreach (var page in commandSelection) {
                    var dict = new Dictionary<string, string>();
                    FilterPageDataProperties(page, dict, filter);
                    pages.Add(DictToJson(dict));
                }

                json = pages.Count > 1 ? string.Format("[ {0} ]", string.Join(",", pages)) : pages.FirstOrDefault();

                pb.Response.ContentType = "application/json";
                pb.Response.Write(json);
                pb.Response.End();

            }
        }
    }

}