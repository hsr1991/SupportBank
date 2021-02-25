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
    class Accounts
    {
       public string AccountName {get; set;}

       public List<Transaction> IncomingTransaction {get; set;}

       public List<Transaction> OutgoingTransaction {get; set;}

       public double NetDebt {get; set;}
    }
}
