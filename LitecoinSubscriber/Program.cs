using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using LitecoinSubscriber.Model;
using NBitcoin;
using NBitcoin.Litecoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace LitecoinSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            ZeroMQClient(args);
        }

        public static void ZeroMQClient(string[] args)
        {
            Networks.EnsureRegistered();
            
            using (var subscriber = new ZSocket(ZSocketType.SUB))
            {
                string connect_to = "tcp://127.0.0.1:28332";
                Console.WriteLine("I: Connecting to {0}...", connect_to);
                subscriber.Connect(connect_to);

                // Subscribe to topics

                subscriber.Subscribe("hashtx");
                Console.WriteLine("I: Subscribing to topic hashtx...");
                subscriber.Subscribe("hashblock");
                Console.WriteLine("I: Subscribing to topic hashblock...");
                subscriber.Subscribe("rawblock");
                Console.WriteLine("I: Subscribing to topic rawblock...");
                subscriber.Subscribe("rawtx");
                Console.WriteLine("I: Subscribing to topic rawtx...");

                Console.WriteLine("Subscribed.. Waiting for messages.");

                while (true)
                {

                    using (var replyFrame = subscriber.ReceiveMessage())

                    {
                        int messageNumber = 0;
                        foreach (var item in replyFrame)
                        {
                            byte[] message = item.Read();

                            if (messageNumber == 0)
                            {
                                Console.WriteLine(messageNumber + " ---");
                                Console.WriteLine(replyFrame[0]);

                            }
                            else
                            {
                                Console.WriteLine(messageNumber + " ---");
                                Console.WriteLine(ByteArrayToString(message));
                                //DO JOBS

                                if (replyFrame[0].ToString().Equals("rawtx") && messageNumber == 1)
                                {
                                    Console.WriteLine("***rawtx***");
                                    string code = ByteArrayToString(message);
                                    Console.WriteLine(code);
                                    GetTransactionModel(code);
                                }


                            }
                            messageNumber++;
                        }

                        Console.WriteLine("son ---");


                    }
                    // Console.ReadKey();
                }




            }
        }


        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        
        public static RawTransaction GetTransactionModel(string code)
        {
            RawTransaction model = new RawTransaction();
            Transaction tx = new Transaction(code);

            //var inputs = tx.Inputs;
            //foreach (TxIn input in inputs)
            //{
            //    OutPoint previousOutpoint = input.PrevOut;

            //    Console.WriteLine(previousOutpoint.Hash); // hash of prev tx
            //    Console.WriteLine(previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
            //    Console.WriteLine();
            //}



            var outputs = tx.Outputs;
            foreach (TxOut output in outputs)
            {
                Money amount = output.Value;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = output.ScriptPubKey;
                Console.WriteLine(paymentScript);  // It's the ScriptPubKey
                var testNetworkAdress = paymentScript.GetDestinationAddress(Network.TestNet);
                Console.WriteLine(testNetworkAdress);
                //var mainNetworkAddress = paymentScript.GetDestinationAddress(Network.Main);
                //Console.WriteLine(mainNetworkAddress);
                Console.WriteLine();
            }
           

            return model;
        }





        //public static void DecodeRawTx(string val)
        //{
        //     const string path = "C:\\Program Files\\Litecoin\\daemon\\litecoin-cli.exe";

        //    // Use ProcessStartInfo class
        //    ProcessStartInfo startInfo = new ProcessStartInfo();
        //    startInfo.CreateNoWindow = false;
        //    startInfo.UseShellExecute = false;
        //    startInfo.FileName = "litecoin-cli.exe";
        //    startInfo.WindowStyle = ProcessWindowStyle.Normal;
        //    startInfo.Arguments =  path + string.Concat("decoderawtransaction ", val);

        //    try
        //    {
        //        // Start the process with the info we specified.
        //        // Call WaitForExit and then the using statement will close.
        //        using (Process exeProcess = Process.Start(startInfo))
        //        {
        //            exeProcess.WaitForExit();
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        // Log error.
        //    }
        //}

    }
}
