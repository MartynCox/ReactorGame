﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace Assets.Scripts.Settings
{
    [Preserve]
    [Serializable]
    public class ScenarioResult
    {
        [JsonProperty("records")]
        public List<CycleResult> Records;

        public ScenarioResult()
        {
            Records = new List<CycleResult>();
        }

        public ScenarioResult(List<CycleResult> records)
        {
            Records = records;
        }

        public void AddRecord(CycleResult record)
        {
            Records.Add(record);
        }

        public string ToCSV()
        {
            StringBuilder sb = new StringBuilder();

            // Write the header
            sb.AppendLine(CycleResult.GetHeader());
            foreach (CycleResult record in Records)
            {
                sb.AppendLine(record.ToCSV());
            }
            return sb.ToString();
        }
    }
}
