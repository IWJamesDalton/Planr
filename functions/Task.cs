using System;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace Planr.Tasks
{
    public class Task
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; }
        [JsonProperty(PropertyName = "pomodoro")]
        public int Pomodoro { get; set; }
    }
}