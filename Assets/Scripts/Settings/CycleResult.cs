using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Settings
{
    [Serializable]
    public class CycleResult
    {
        [JsonProperty("valvesFlows")]
        public List<int> ValveFlows { get; set;}
        [JsonProperty("temperature")]
        public float ReactorTemperature { get; set; }
        [JsonProperty("time")]
        public int Time { get; set; }

        public CycleResult()
        {
            ValveFlows = new List<int>();
            ReactorTemperature = 0;
            Time = 0;
        }

        public CycleResult(List<int> valveFlows, float reactorTemperature, int time)
        {
            ValveFlows = valveFlows;
            ReactorTemperature = reactorTemperature;
            Time = time;
        }

        public static string GetHeader()
        {
            return "Time, Temperature, Valve1, Valve2, Valve3, Valve4, Valve5, Valve 6,"
                + "Valve 7, Valve 8, Valve 9, Valve 10, Valve 11, Valve 12, Valve 13";
        }

        public string ToCSV()
        {
            return Time.ToString() + "," + ReactorTemperature.ToString()
                + "," + string.Join(",", ValveFlows);
        }
    }
}