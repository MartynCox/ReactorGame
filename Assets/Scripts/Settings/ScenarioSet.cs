using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

namespace Assets.Scripts.Settings
{
    [Serializable]
    [Preserve]
    public class ScenarioSet
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scenarios")]
        public List<GameScenario> Scenarios { get; set; }

        [JsonProperty("videoUrls")]
        public List<string> VideoUrls { get; set; }

        [JsonProperty("allowVideoSkipping")]
        public bool AllowVideoSkipping { get; set; }

        public ScenarioSet()
        {
            Scenarios = new List<GameScenario>();
            VideoUrls = new List<string>();
            Name = "New Settings";
        }

        public static ScenarioSet LoadSettings(string json)
        {
            ScenarioSet gameSettings = JsonConvert.DeserializeObject<ScenarioSet>(json);
            if (gameSettings == null)
            {
                Console.WriteLine("Deserialization failed");
                throw new JsonSerializationException();
            }
            return gameSettings;
        }

        public void SaveSettings(string fname)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(fname, json);
        }
    }
}
