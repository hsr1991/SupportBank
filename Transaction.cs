using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;


namespace SupportBank
{
    // private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    class Transaction
    {
         [JsonProperty("Date")]
        public DateTime TransactionDate {get; set;}
        [JsonProperty("FromAccount")]
        public string FromName {get; set;}
        [JsonProperty("ToAccount")]
        public string ToName {get; set;}
        [JsonProperty("Narrative")]
        public string Narrative {get; set;}
        [JsonProperty("Amount")]
        public double Amount {get; set;}
    }
}
