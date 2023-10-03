using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

[System.Serializable]
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

    public static GameSettings LoadSettings(String fname)
    {
        string json = System.IO.File.ReadAllText(fname);
        GameSettings settings = JsonConvert.DeserializeObject<GameSettings>(json);
        return settings;
    }
}

[System.Serializable]
public class TankSettings
{
    [JsonProperty("capacity")]
    public int Capacity { get; set; }
    [JsonProperty("startLevel")]
    public int StartLevel { get; set; }
}