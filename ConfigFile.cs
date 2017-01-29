using System;
using System.Collections.Generic;
using System.IO;

namespace Kraken
{
    /// <summary>
    /// A simple and crappy configuration file class.
    /// </summary>
    class ConfigFile
    {
        private FileStream file;
        private Dictionary<string, string> entries = new Dictionary<string, string>();


        /// <summary>
        /// Constructs a ConfigFile object, loading configuration from specified file
        /// </summary>
        public ConfigFile(string filename)
        {
            file = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            StreamReader sr = new StreamReader(file);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (!line.Contains("="))
                    continue;
                string[] arr = line.Split("=".ToCharArray(), 2);

                entries[arr[0]] = arr[1];
            }
        }

        /// <summary>
        /// Returns an integer from the configuration
        /// </summary>
        public int GetInt(string key)
        {
            if (!entries.ContainsKey(key))
                return -1;
            string val = entries[key];
            return Int32.Parse(val);
        }

        /// <summary>
        /// Sets the integer value of an entry in the configuration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetInt(string key, int value)
        {
            entries[key] = value.ToString();
        }

        /// <summary>
        /// Writes configuration back to the originally loaded file
        /// </summary>
        public void Save()
        {
            StreamWriter sw = new StreamWriter(file);

            file.SetLength(0);

            foreach (KeyValuePair<string, string> kvp in entries)
            {
                sw.WriteLine(kvp.Key + "=" + kvp.Value);
            }

            sw.Flush();
        }

        /// <summary>
        /// Closes associated file.
        /// </summary>
        public void Close()
        {
            file.Close();
        }
    }
}
