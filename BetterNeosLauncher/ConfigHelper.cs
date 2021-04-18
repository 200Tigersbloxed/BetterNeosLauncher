using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace BetterNeosLauncher
{
    class ConfigHelper
    {
        public static string ConfigString = String.Empty;
        public static Config config;
        /*
        public static JSchema schema = JSchema.Parse(@"{
            'type': 'object',
            'properties': {
                'DataPath': {'type':'string'},
                'CachePath': {'type':'string'},
                'SRanipalPath': {'type':'string'},
                'NeosPath': {'type':'string'},
            }
        }");
        */

        public static async Task<string> ReadConfigFile(string configLocation = "bnlconfig.json")
        {
            if (File.Exists(configLocation))
            {
                using (var reader = File.OpenText(configLocation))
                {
                    ConfigString = await reader.ReadToEndAsync();
                    return ConfigString;
                }
            }
            else
            {
                // create it
                Config ctc = new Config();
                string ConfigString = JsonConvert.SerializeObject(ctc);
                await SaveConfig(ctc, configLocation);
                return ConfigString;
            }
        }

        public static Config LoadConfig(string cs)
        {
            config = JsonConvert.DeserializeObject<Config>(cs);
            return config;
        }

        public static async Task SaveConfig(Config configToSave, string configLocation = "bnlconfig.json")
        {
            string jsonToWrite = JsonConvert.SerializeObject(configToSave);
            using (StreamWriter outputFile = new StreamWriter(configLocation))
            {
                await outputFile.WriteAsync(jsonToWrite);
            }
        }

        public static bool VerifyConfig(Config configToVerify)
        {
            return true;
            // Will be done later
            /*
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Config));
            JObject co = JObject.Parse(JsonConvert.SerializeObject(configToVerify));
            return co.IsValid(schema);
            */
        }
    }

    class Config
    {
        public string DataPath = String.Empty;
        public string CachePath = String.Empty;
        public string SRanipalPath = String.Empty;
        public string NeosPath = String.Empty;
        public string OtherLaunchOptions = String.Empty;
    }
}
