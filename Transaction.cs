using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;


namespace SupportBank
{
    // private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    class Transaction
    {
        public DateTime TransactionDate {get; set;}
        public string FromName {get; set;}
        public string ToName {get; set;}
        public string Narrative {get; set;}
        public double Amount {get; set;}
    }
}
