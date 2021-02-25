using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;


namespace SupportBank
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            Logger.Error("testing my logger");
            Logger.Info("blah");

            string path = @"support-bank-resources\Transactions2014.csv";
            var readText = File.ReadAllLines(path).ToList().Skip(1);
            List<Transaction> tList = new List<Transaction>();
            List<string> nameList = new List<string>();
            foreach (string line in readText)
            {
                string[] data = line.Split(',');
                nameList.Add(data[1]);
                nameList.Add(data[2]);
                var transaction = new Transaction
                {
                    TransactionDate = DateTime.Parse(data[0]),
                    FromName = data[1],
                    ToName = data[2],
                    Narrative = data[3],
                    Amount = double.Parse(data[4])
                };
                tList.Add(transaction);
            }

            List<Accounts> accountsList = new List<Accounts>();
            List<string> uniqueNameList = nameList.Distinct().ToList();
            foreach (var name in uniqueNameList)
            {
                List<Transaction> incomingTransactionList = new List<Transaction>();
                List<Transaction> outgoingTransactionList = new List<Transaction>();
                double netDebt = 0;
                foreach (var item in tList)
                {
                    if (item.ToName == name)
                    {
                        incomingTransactionList.Add(item);
                        netDebt += item.Amount;
                    }

                    if (item.FromName == name)
                    {
                        outgoingTransactionList.Add(item);
                        netDebt -= item.Amount;
                    }
                }
                var account = new Accounts
                {
                    AccountName = name,
                    IncomingTransaction = incomingTransactionList,
                    OutgoingTransaction = outgoingTransactionList,
                    NetDebt = netDebt

                };
                accountsList.Add(account);
            }

            Console.Write("What information would you like? \n");
            Console.Write("To print out the names of each person, along with the total amount they owe or are owed, type 'List All'. \n");
            Console.Write("To print out just one user account, with all transactions associated with it, type 'List[Account]'. ");
            string selection = Console.ReadLine();

            if (selection == "List[Account]")
            {
                Console.Write("Please type in the name of the account you want to display e.g. 'Dan W'. ");
                string inputName = Console.ReadLine();
                foreach (var acc in accountsList)
                {
                    if (acc.AccountName == inputName)
                    {
                        Console.WriteLine("Account Name: " + acc.AccountName);
                        Console.WriteLine("-----------------");
                        Console.WriteLine("Incoming Transactions List: "); //we can't console.writeline a list so we have to do
                        //a foreach to console each item in list.
                        Console.WriteLine("-----------------");
                        foreach (var i in acc.IncomingTransaction)
                        {
                            Console.WriteLine(" Transaction Date: " + i.TransactionDate.ToShortDateString());
                            Console.WriteLine(" From Name: " + i.FromName);
                            Console.WriteLine(" To Name: " + i.ToName);
                            Console.WriteLine(" Narrative: " + i.Narrative);
                            Console.WriteLine(" Amount: " + i.Amount);
                            Console.WriteLine("-----------------");
                        }
                        Console.WriteLine("Outgoing Transactions List: ");
                        Console.WriteLine("-----------------");
                        foreach (var i in acc.OutgoingTransaction)
                        {
                            Console.WriteLine(" Transaction Date: " + i.TransactionDate.ToShortDateString());
                            Console.WriteLine(" From Name: " + i.FromName);
                            Console.WriteLine(" To Name: " + i.ToName);
                            Console.WriteLine(" Narrative: " + i.Narrative);
                            Console.WriteLine(" Amount: " + i.Amount);
                            Console.WriteLine("-----------------");
                        }
                        Console.WriteLine("Net Debt: " + acc.NetDebt);
                        Console.WriteLine("-----------------");
                    }
                }
            }

            if (selection == "List All")
            {
                foreach (var acc in accountsList)
                {
                    Console.WriteLine("Name:" + acc.AccountName);
                    Console.WriteLine(" Net Debt: " + acc.NetDebt);
                    Console.WriteLine("-----------------");
                }
            }
        }
    }
}
