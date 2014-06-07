using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace Nyangoro.Core.Host
{
    static class Config
    {
        private static Hashtable conf;

        //adds config variables to the conf hashtable
        //@TODO use config files later
        public static void buildConfig()
        {
            Config.conf = new Hashtable();

            Config.conf["working_dir"] = Directory.GetCurrentDirectory();
        }

        public static string Get(string key)
        {
            if(Config.conf.ContainsKey(key))
                return (string)Config.conf[key];
            else
                return "";
        }
    }
}
