using System;
using GStore;
using Grpc.Core;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string id = null, URL = null, otherURL = null, host = null;
            Int64 min_delay = 0, max_delay = 0;
            int port = 0;

            if (args.Length != 4 && args.Length != 5) 
            {
                Console.WriteLine("Wrong format. Should be: id url min_delay max_delay [otherURL]\n otherURL is an optional argument used to connect to a pre-existing network.");    
            }
            
            id = args[0];
            URL = args[1];
            min_delay = Int64.Parse(args[2]);
            max_delay = Int64.Parse(args[3]);
            
            string[] details = URL.Split("//");
            details = details[1].Split(':');

            host = details[0];
            port = int.Parse(details[1]);
           
            if (args.Length == 5) 
            {
                otherURL = args[4];
            }

            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { GStore.GStore.BindService(new GStoreService(id, URL, otherURL))},
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("GStore server running on " + host + " listening on port " + port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
