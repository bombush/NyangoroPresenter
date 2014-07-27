using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace Nyangoro.Core.Host
{
    //@TODO: make into service and put ALL config in here
    static class Config
    {
        private static Hashtable conf;

        //adds config variables to the conf hashtable
        //@TODO use config files later
        public static void BuildConfig()
        {
            Config.conf = new Hashtable();

            Config.conf["working_dir"] = Directory.GetCurrentDirectory();
            Config.conf["plugins_root"] = Config.conf["working_dir"]+"\\plugins\\";
        }

        public static string Get(string key)
        {
            if(Config.conf.ContainsKey(key))
                return (string)Config.conf[key];
            else
                return "";
        }

        public static string GetPluginDir(string pluginName)
        {
            string pluginDir = Config.conf["plugins_root"] + "\\" + pluginName + "\\";
            return pluginDir;
        }
    }
}
