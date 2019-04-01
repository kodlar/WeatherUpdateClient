using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherUpdateClient.Model;
using ZeroMQ;

namespace WeatherUpdateClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WUClient(args);
        }

        public static void WUClient(string[] args)
        {
            //
            // Weather update client
            // Connects SUB socket to tcp://localhost:5556
            // Collects weather updates and finds avg temp in zipcode
            //
            // Author: metadings
            //

            if (args == null || args.Length < 2)
            {
                Console.WriteLine();
                Console.WriteLine("Usage: ./{0} WUClient [ZipCode] [Endpoint]", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine();
                Console.WriteLine("    ZipCode   The zip code to subscribe. Default is 72622 Nürtingen");
                Console.WriteLine("    Endpoint  Where WUClient should connect to.");
                Console.WriteLine("              Default is tcp://127.0.0.1:5556");
                Console.WriteLine();
                if (args.Length < 1)
                    args = new string[] { "72622", "tcp://127.0.0.1:5556" };
                else
                    args = new string[] { args[0], "tcp://127.0.0.1:5556" };
            }

            // Socket to talk to server
            // using (var context = new ZContext())
            using (var subscriber = new ZSocket(ZSocketType.SUB))
            {
                string connect_to = args[1];
                Console.WriteLine("I: Connecting to {0}...", connect_to);
                subscriber.Connect(connect_to);

                // Subscribe to zipcode
                string zipCode = args[0];
                Console.WriteLine("I: Subscribing to zip code {0}...", zipCode);
                subscriber.Subscribe(zipCode);

                // Process 10 updates
                int i = 1;
                long total_temperature = 0;
                //for (; i < 20; ++i)
                while(true)
                {
                    using (var replyFrame = subscriber.ReceiveFrame())
                    {
                        string reply = replyFrame.ReadString();
                        var json = reply.Split('*')[1];
                        //Console.WriteLine(reply);
                        //Console.WriteLine(json);
                     
                        WeatherItem wi = JsonConvert.DeserializeObject<WeatherItem>(json);
                        Console.WriteLine(wi.City);
                        Console.WriteLine(wi.Id);
                        Console.WriteLine(wi.Status);
                        Console.WriteLine(wi.Temperature);
                        //Console.ReadKey();
                        //total_temperature += Convert.ToInt64(reply.Split(' ')[1]);
                        Console.WriteLine("Average temperature for zipcode '{0}' was {1}°", zipCode, (total_temperature / i));
                        i++;
                    }
                }

                Console.WriteLine("Average temperature for zipcode '{0}' was {1}°", zipCode, (total_temperature / i));
                Console.ReadKey();
            }
        }
    }
}
