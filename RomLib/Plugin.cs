using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/*
 * Plugin-related classes
 */

namespace PKHack
{
    /// <summary>
    /// PK Hack plugins must have a main class that implements this interface.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// A descriptive name for the plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called to instruct a plugin to perform any initialization it requires
        /// </summary>
        void Initialize();


    }


    /// <summary>
    /// A static class for holding all registered plugins
    /// </summary>
    public static class Plugins
    {
        private static List<IPlugin> plugins
            = new List<IPlugin>();

        public static IPlugin GetPlugin(int index)
        {
            return plugins[index];
        }


        public static void LoadPluginDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new Exception("Error loading plugins directory: path invalid.");

            string[] files = Directory.GetFiles(path, "*.dll");

            try
            {
                foreach (string s in files)
                    LoadPlugin(s);
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        /// <summary>
        /// Loads a plugin DLL
        /// </summary>
        /// <param name="path">The path to the plugin DLL.</param>
        public static void LoadPlugin(string path)
        {
            Assembly a = Assembly.LoadFile(path);

            Type pluginType = null;

            // Find an a type that implements IPlugin
            foreach (Type type in a.GetTypes())
            {
                if (type.GetInterface("IPlugin") != null)
                {
                    if (pluginType != null)
                        throw new Exception("A plugin assembly should contain only one class that implements IPlugin!");
                    pluginType = type;
                }
            }
            if (pluginType == null)
                throw new Exception("A plugin assembly must contain a class implementing IPlugin!");

            // Create an instance of the type
            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);

            // Add it to the list of available plugins
            plugins.Add(plugin);

            // Initialize the plugin
            plugin.Initialize();
        }
    }
}
