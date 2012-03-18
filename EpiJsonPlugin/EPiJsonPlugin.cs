//Work continued on Allan's blogpost from 2009
//http://labs.episerver.com/en/Blogs/Allan/Dates/2009/3/Output-PageData-as-JSON/

using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly Dictionary<string, ICommandTemplate> CommandDict = new Dictionary<string, ICommandTemplate>();
        private static readonly Dictionary<Type, ITypeMapTemplate> TypeMapDict = new Dictionary<Type, ITypeMapTemplate>();

        public static void Initialize(int bitflags)
        {
            PageBase.PageSetup += PageBaseBaseSetup;
        }

        static void PageBaseBaseSetup(PageBase sender, PageSetupEventArgs e)
        {
            sender.Load += SenderLoad;
        }



        static void FilterPageDataProperties(PageData pd, Dictionary<string, string> dict, string[] onlyProps = null)
        {

            foreach (PropertyData prop in pd.Property)
            {
                //Filter out properties if told to
                if (onlyProps != null && !onlyProps.Contains(prop.Name))
                {
                    continue;
                }

                ITypeMapTemplate mapTemplate;

                if (!TypeMapDict.TryGetValue(prop.GetType(), out mapTemplate)) continue;

                var propval = mapTemplate.Map(pd, prop);

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

        static void SenderLoad(object sender, EventArgs e)
        {
            var pb = sender as PageBase;


            if (pb == null) return;
            //Check if json was requested
            if (string.IsNullOrEmpty(pb.Request["json"])) return;

            //Parse option

            var command = pb.Request["json"].ToLower();

            ICommandTemplate cmd;

            if (!CommandDict.TryGetValue(command, out cmd)) { 
                pb.Response.End();
                return;
            }

            if(cmd == null)
            {
                throw new InvalidOperationException(string.Format("Could not load handler for command {0}",command));
            }

            var commandSelection = cmd.ExecuteCommand(pb);
            var filter = cmd.GetPropertyFilter();
            var pages = new List<string>();

            foreach (var page in commandSelection)
            {
                var dict = new Dictionary<string, string>();
                FilterPageDataProperties(page, dict, filter);
                pages.Add(DictToJson(dict));
            }

            var json = pages.Count > 1 ? string.Format("[ {0} ]", string.Join(",", pages)) : pages.FirstOrDefault();

            pb.Response.ContentType = "application/json";
            pb.Response.Write(json ?? string.Empty);

            pb.Response.End();
        }
    }

}