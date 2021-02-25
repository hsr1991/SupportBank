using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;


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

            Logger.Info("\n ----------------- \n New Run: ");

            Console.Write("What type of file type are you using? Please type either 'CSV' or 'JSON'. \n");
            string filetype = Console.ReadLine();
            while (filetype != "CSV" && filetype !="JSON") 
            {
                Logger.Error("Invalid selection. '" + filetype + "' is an invalid file type. User prompted to retry.");
                Console.WriteLine("Invalid selection. '" + filetype + "' is an invalid file type.");
                filetype = Console.ReadLine();
            }
            List<Transaction> tList = new List<Transaction>();
            List<string> nameList = new List<string>();
            if (filetype=="JSON")
            {
                string input = @"support-bank-resources\Transactions2013.json";
                var jsonString = File.ReadAllText(input);
                tList = JsonConvert.DeserializeObject<List<Transaction>>(jsonString);
                foreach(var trans in tList)
                {
                    nameList.Add(trans.FromName);
                    nameList.Add(trans.ToName);
                }
               
            }
            
            if (filetype=="CSV")
            {
                string path = @"support-bank-resources\DodgyTransactions2015.csv";
                // string path = @"support-bank-resources\Transactions2014.csv";
                var readText = File.ReadAllLines(path).ToList().Skip(1);
                
                int linecounter=1;
                foreach (string line in readText)
                {
                    linecounter+=1;
                    string[] data = line.Split(',');
                    nameList.Add(data[1]);
                    nameList.Add(data[2]);
                    if (DateTime.TryParse(data[0], out DateTime variable)==false)
                    {
                        Logger.Error("\n Line"+ linecounter+": '" + line +"' - Transaction Date is not in a valid Date Time format.\n Skipped Line.");
                        if (double.TryParse(data[4], out double numberZ)== false)
                            {
                                Logger.Error("\n Line"+ linecounter+": '"+ line + "'  - Amount is not in a valid format.\n Skipped Line.");
                            }
                        continue; //skips to the next iteration without finishing this iteration
                    }
                    if (double.TryParse(data[4], out double numberY)== false)
                    {
                        Logger.Error("\n Line"+ linecounter+": '"+ line + "'  - Amount is not in a valid format.\n Skipped Line.");
                        continue;
                    }
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
            Console.Write("To print out just one user account, with all transactions associated with it, type 'List Account'. ");
            string selection = Console.ReadLine();
            while (selection != "List Account" && selection != "List All") 
            {
                Logger.Error("Invalid selection. '" + selection + "' is an invalid selection. User prompted to retry.");
                Console.WriteLine("Invalid selection. '" + selection + "' is an invalid selection.");
                selection = Console.ReadLine();
            }
            if (selection == "List Account")
            {
                Console.Write("Please type in the name of the account you want to display e.g. 'Dan W'. ");
                string inputName = Console.ReadLine();

                while (uniqueNameList.Contains(inputName)==false) //use while instead of if so you can put in a wrong name as
                // many times as you want. if will only let you do it once.
                {
                    Logger.Error(inputName + " is not a valid account name. User prompted to retry.");
                    Console.WriteLine(inputName + " is not a valid account name. Please try again.");
                    inputName = Console.ReadLine();
                }
              
                
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
