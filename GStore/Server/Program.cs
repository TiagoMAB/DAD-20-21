using System;
using GStore;
using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = null, serverHost = null;
            int port, serverPort;

            if (args.Length != 2 && args.Length != 4) 
            {
                Console.WriteLine("Wrong format. Should be: host port [serverHost serverPort]\n serverHost and serverPort are optional arguments used to connect to a pre-existing network.");    
            }
            
            host = args[0];
            port = int.Parse(args[1]);
            
            if (args.Length == 4) 
            {
                serverHost = args[2];
                serverPort = int.Parse(args[3]);
            }

            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { GStore.GStore.BindService(new GStoreService()) },
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
