//Work continued on Allan's blogpost from 2009
//http://labs.episerver.com/en/Blogs/Allan/Dates/2009/3/Output-PageData-as-JSON/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using EPiServer;
using EPiServer.PlugIn;
using EPiServer.Core;
using EpiJsonPlugin.TypeMaps;
using EpiJsonPlugin.Commands;

namespace EpiJsonPlugin
{
    [PagePlugIn(DisplayName = "JSON Exporter", Description = "Exports pages as JSON")]
    public class EPiJsonPlugin
    {
        private static readonly object Lock = new object();

        static EPiJsonPlugin()
        {
            lock(Lock) {
                LoadTypeMaps();
                LoadCommands();
            }
        }

        /// <summary>
        /// Loads all types having the TypeMap Attribute from loaded assemblies
        /// </summary>
        private static void LoadTypeMaps()
        {
            var assemblies = Utils.GetLoadedAssemblies();
            var types = new List<Type>();

            foreach (var typesInAsm in assemblies.Select(Utils.GetTypesWithAttribute<TypeMapAttribute>))
            {
                types.AddRange(typesInAsm);
            }

            //Activate types
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var attribs = type.GetCustomAttributes(typeof(TypeMapAttribute), true);
                var typeMapAttribute = attribs[0] as TypeMapAttribute;
                if (typeMapAttribute == null) continue;

                var propertyType = typeMapAttribute.PropertyType;

                //Overwrite type map redeclarations
                if (TypeMapDict.ContainsKey(propertyType))
                {
                    TypeMapDict.Remove(propertyType);
                }

                TypeMapDict.Add(propertyType, instance as ITypeMapTemplate);
            }
        }

        /// <summary>
        /// Loads all types having the Command attribute from loaded assemblies
        /// </summary>
        private static void LoadCommands()
        {
            var assemblies = Utils.GetLoadedAssemblies();
            var types = new List<Type>();

            foreach (var typesInAsm in assemblies.Select(Utils.GetTypesWithAttribute<CommandAttribute>))
            {
                types.AddRange(typesInAsm);
            }

            //Activate types
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var attribs = type.GetCustomAttributes(typeof(CommandAttribute), true);
                var commandAttribute = attribs[0] as CommandAttribute;
                if (commandAttribute == null) continue;
                var cmdName = commandAttribute.CommandName;

                //Overwrite redeclaration of cmdname
                if (CommandDict.ContainsKey(cmdName))
                {
                    CommandDict.Remove(cmdName);
                }
                CommandDict.Add(cmdName, instance as ICommandTemplate);
            }
        }

        //Loaded commands
        private static readonly Dictionary<string, ICommandTemplate> CommandDict = new Dictionary<string, ICommandTemplate>();
        
        //Loaded type maps
        private static readonly Dictionary<Type, ITypeMapTemplate> TypeMapDict = new Dictionary<Type, ITypeMapTemplate>();

        public static void Initialize(int bitflags)
        {
            PageBase.PageSetup += PageBaseBaseSetup;
        }

        static void PageBaseBaseSetup(PageBase sender, PageSetupEventArgs e)
        {
            sender.Load += SenderLoad;
        }

        /// <summary>
        /// Generates JSON key-value pairs
        /// </summary>
        /// <param name="pd">Page to be serialized</param>
        /// <param name="dict">Dictionary to fill with values</param>
        /// <param name="onlyProps">Only serialized these properties if provided</param>
        static void FilterPageDataProperties(PageData pd, IDictionary<string, string> dict, IEnumerable<string> onlyProps = null)
        {
            foreach (var prop in pd.Property.Where(prop => onlyProps == null || onlyProps.Contains(prop.Name)))
            {
                ITypeMapTemplate mapTemplate;

                if (!TypeMapDict.TryGetValue(prop.GetType(), out mapTemplate)) continue;

                //Handler found but no instance registered (occurs when not implementing ITypeMapTemplate interface properly)
                if (mapTemplate == null)
                {
                    throw new InvalidOperationException(string.Format("Could not load map for type {0}, make sure type maps inherit from ITypeMapTemplate", prop.GetType()));
                }

                var propval = mapTemplate.Map(pd, prop);

                if (!string.IsNullOrEmpty(propval))
                {
                    dict.Add(prop.Name, propval);
                }
            }
        }

        /// <summary>
        /// Creates a JSON string from a Dictionary
        /// </summary>
        /// <param name="dict">Dictionary to serialize</param>
        /// <returns>A JSON string</returns>
        static string DictToJson(Dictionary<string, string> dict)
        {
            var contents = string.Join(",", dict.Select(d => string.Format("\"{0}\" : {1}", d.Key, d.Value)));
            return String.Format("{{ {0} }}", contents);
        }

        static void SenderLoad(object sender, EventArgs e)
        {
            var pb = sender as PageBase;

            if (pb == null) return;

            //Check if json was requested
            if (string.IsNullOrEmpty(pb.Request["json"])) return;

            //get command
            var command = pb.Request["json"].ToLower();

            //Check cache for page/command compound key
            var cachedJson = CacheManager.RuntimeCacheGet(CacheKey(pb, command)) as string;

            if(cachedJson != null)
            {
                EndJson(pb,cachedJson);
            }

            ICommandTemplate cmd;

            //Try to find a handler for provided command
            if (!CommandDict.TryGetValue(command, out cmd)) { 
                //End response if no handler found
                pb.Response.End();
                return;
            }

            //Handler found but no instance registered (occurs when not implementing ICommandTemplate interface properly)
            if(cmd == null)
            {
                throw new InvalidOperationException(string.Format("Could not load handler for command {0}, make sure commands inherit from ICommandTemplate",command));
            }

            //Execute command
            var commandSelection = cmd.ExecuteCommand(pb);
            var filter = cmd.GetPropertyFilter();

            //Process command result
            var pages = new List<string>();

            

            foreach (var page in commandSelection)
            {
                var dict = new Dictionary<string, string>();
                FilterPageDataProperties(page, dict, filter);
                pages.Add(DictToJson(dict));
            }

            //If multiple pages, wrap in array
            var multipage = pages.Count > 1;
            var json = multipage ? string.Format("[ {0} ]", string.Join(",", pages)) : pages.FirstOrDefault();

            //Cache resultset
            if(json != null) {
                var dependency = multipage ? new MultipageCacheDependency(commandSelection) : DataFactoryCache.CreateDependency(pb.CurrentPageLink);
                CacheManager.RuntimeCacheInsert(CacheKey(pb, command), json, dependency);
            }

            EndJson(pb, json);
        }

        private static void EndJson(Page pb, string json)
        {
            //Return json
            pb.Response.ContentType = "application/json";
            pb.Response.Write(json ?? string.Empty);

            pb.Response.End();
        }


        private static string CacheKey(PageBase pb, string cmd)
        {
            return string.Format("{0}:{1}", pb.CurrentPageLink, cmd);
        }
    }


}