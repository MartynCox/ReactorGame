using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Scripting;

[Serializable]
[Preserve]
public class GameSettings
{
    [JsonProperty("cycleDuration")]
    public int CycleDuration { get; set; }
    [JsonProperty("breakTankOnOverflow")]
    public bool BreakTankOnOverflow { get; set; }
    [JsonProperty("flowTemperatures")]
    public Dictionary<int, int> FlowTemperatures { get; set; }
    [JsonProperty("tanks")]
    public Dictionary<string, TankSettings> Tanks { get; set; }
    [JsonProperty("targetTemperature")]
    public int TargetTemperature { get; set; }
    [JsonProperty("valves")]
    public Dictionary<string, ValveSettings> Valves { get; set; }
    
    public static GameSettings LoadSettings(String fname)
    {
        try
        {
            string json = System.IO.File.ReadAllText(fname);
            GameSettings settings = JsonConvert.DeserializeObject<GameSettings>(json);
            return settings;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Settings file not found: " + fname);
            return null;
        }
    }

    public void SaveSettings(String fname)
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        System.IO.File.WriteAllText(fname, json);
    }
}

[Serializable]
[Preserve]
public class TankSettings
{
    [JsonProperty("capacity")]
    public int Capacity { get; set; }
    [JsonProperty("startLevel")]
    public int StartLevel { get; set; }
}

[Serializable]
[Preserve]
public class ValveSettings
{
    [JsonProperty("maxFlowDisplay")]
    public int MaxFlowDisplay { get; set; }
    [JsonProperty("flowStepSize")]
    public int FlowStepSize { get; set; }
    [JsonProperty("flowRatePerStep")]
    public float FlowRatePerStep { get; set; }
    [JsonProperty("isBroken")]
    public bool IsBroken { get; set; }
}