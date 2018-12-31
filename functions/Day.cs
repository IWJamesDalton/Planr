using System;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace Planr.Tasks
{
        public class Day
    {
        [JsonProperty(PropertyName = "id")]
        public string Id {get;set;}
        [JsonProperty(PropertyName = "date")]
        public DateTime Date {get;set;}
        [JsonProperty(PropertyName = "notes")]
        public string Notes {get;set;}
        [JsonProperty(PropertyName = "userId")]
        public string UserId {get;set;}
        [JsonProperty(PropertyName = "productivityScore")]
        public int ProductivityScore {get;set;}
        [JsonProperty(PropertyName = "tasks")]
        public List<Planr.Tasks.Task> Tasks {get;set;}

    }
}