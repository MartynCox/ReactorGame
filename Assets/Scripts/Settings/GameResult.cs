﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Assets.Scripts.Settings
{
    [Serializable]
    [Preserve]
    public class GameResult
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("gameName")]
        public string GameName { get; set; }

        [JsonProperty("videoShown")]
        public string VideoShown { get; set; }

        [JsonProperty("startTimestamp")]
        public string StartTimestamp { get; set; }

        [JsonProperty("endTimestamp")]
        public string EndTimestamp { get; set; }

        [JsonProperty("completionCode")]
        public string CompletionCode { get; set; }

        [JsonProperty("results")]
        public List<ScenarioResult> ResultList { get; set; }

        [JsonProperty("csvHeaders")]
        public string CsvHeaders { get; set; }

        public GameResult()
        {
            GameName = "Default";
            CsvHeaders = CycleResult.GetHeader();
            StartTimestamp = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");
            EndTimestamp = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss");
            CompletionCode = "100000";
            ResultList = new List<ScenarioResult>();
        }
    }
}
