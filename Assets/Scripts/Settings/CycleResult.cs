using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Assets.Scripts.Settings
{
    [Serializable]
    [Preserve]
    public class CycleResult
    {
        [JsonProperty("valvesFlows")]
        public List<int> ValveFlows { get; set;}

        [JsonProperty("tankStatus")]
        public List<float> TankStatus { get; set; }

        [JsonProperty("temperature")]
        public float ReactorTemperature { get; set; }

        [JsonProperty("step")]
        public int Step { get; set; }

        [JsonProperty("inGameTime")]
        public float InGameTime { get; set; }

        [JsonProperty("realTime")]
        public float RealTime { get; set; }

        public CycleResult()
        {
            ValveFlows = new List<int>();
            TankStatus = new List<float>();
            ReactorTemperature = 0;
            Step = 0;
            InGameTime = 0;
            RealTime = 0;
        }

        public CycleResult(List<int> valveFlows, List<float> tankStatus, float reactorTemperature, int step, float inGameTime, float realTime)
        {
            ValveFlows = valveFlows;
            TankStatus = tankStatus;
            ReactorTemperature = reactorTemperature;
            Step = step;
            InGameTime = inGameTime;
            RealTime = realTime;
        }

        public static string GetHeader()
        {
            return "Cycle, Temperature, In-game time, Real time, Valve1, Valve2, Valve3, Valve4, Valve5, Valve 6,"
                + "Valve 7, Valve 8, Valve 9, Valve 10, Valve 11, Valve 12, Valve 13, Tank 1, Tank 2, Tank 3, Tank 4";
        }

        public string ToCSV()
        {
            return Step.ToString()
                + "," + ReactorTemperature.ToString()
                + "," + InGameTime.ToString()
                + "," + RealTime.ToString()
                + "," + string.Join(",", ValveFlows)
                + "," + string.Join(",", TankStatus);
        }
    }
}