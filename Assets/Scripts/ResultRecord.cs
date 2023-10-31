using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace Assets.Scripts
{
    [Preserve]
    public class ResultRecord
    {
        private List<int> _valveFlows;
        private float _reactorTemperature;
        private int _time;

        public ResultRecord()
        {
            _valveFlows = new List<int>();
            _reactorTemperature = 0;
            _time = 0;
        }

        public ResultRecord(List<int> valveFlows, int reactorTemperature, int time)
        {
            _valveFlows = valveFlows;
            _reactorTemperature = reactorTemperature;
            _time = time;
        }

        public static string GetHeader()
        {
            return "Time, Temperature, Valve1, Valve2, Valve3, Valve4, Valve5, Valve 6,"
                + "Valve 7, Valve 8, Valve 9, Valve 10, Valve 11, Valve 12, Valve 13";
        }

        public List<int> GetFlows()
        {
            return _valveFlows;
        }

        public void SetFlows(List<int> flows)
        {
            _valveFlows = flows;
        }

        public float GetTemperature()
        {
            return _reactorTemperature;
        }

        public void SetTemperature(float temperature)
        {
            _reactorTemperature = temperature;
        }

        public int GetTime()
        {
            return _time;
        }

        public void SetTime(int time)
        {
            _time = time;
        }

        public string ToCSV()
        {
            return _time.ToString() + "," + _reactorTemperature.ToString()
                + "," + string.Join(",", _valveFlows);
        }
    }
}