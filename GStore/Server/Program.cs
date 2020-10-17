﻿using System;
using GStore;
using Grpc.Core;
using Grpc.Net.Client;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string id = null, URL = null, otherId = null, otherURL = null, host = null;
            Int64 min_delay = 0, max_delay = 0;
            int port = 0;

            if (args.Length != 4 && args.Length != 6) 
            {
                Console.WriteLine("Wrong format. Should be: id url min_delay max_delay [otherId otherURL]\n otherId and otherURL are optional arguments used to connect to a pre-existing network.");
                Console.ReadLine();
                //TO DO: exit ?
            }
            
            id = args[0];
            URL = args[1];
            min_delay = Int64.Parse(args[2]);
            max_delay = Int64.Parse(args[3]);
            string[] details = URL.Split("//");
            details = details[1].Split(':');

            host = details[0];
            port = int.Parse(details[1]);
           
            if (args.Length == 6) 
            {
                otherId = args[4];
                otherURL = args[5];
            }

            Console.WriteLine(id);
            Console.WriteLine(URL);
            Console.WriteLine(otherId);
            Console.WriteLine(otherURL);
            var ServerService = new ServerService(id, URL, otherId, otherURL);
            var gstoreservice = new GStoreService(ServerService);
            var puppetmasterservice = new PuppetMasterService(ServerService);
            var servercommunicationservice = new ServerCommunicationService(ServerService);
            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { GStore.GStore.BindService(gstoreservice), PuppetMaster.BindService(puppetmasterservice), ServerCommunication.BindService(servercommunicationservice) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            server.Start();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(URL);
            PuppetMaster.PuppetMasterClient client = new PuppetMaster.PuppetMasterClient(channel);
            

            Console.WriteLine("GStore server running on " + host + " listening on port " + port);
            Console.WriteLine("Press any key to status the server...");
            Console.ReadKey();
            client.Status(new StatusRequest());
            if (otherURL == null)
            {
                Console.WriteLine("Press any key to partition the server...");
                Console.ReadKey();
                ServerService.partition(new PartitionRequest { Name = "partition1", Ids = { "server1", "server2" } });
                Console.WriteLine("Press any key to write object the server...");
                Console.ReadKey();
                ServerService.write(new WriteRequest { PartitionId = "partition1", ObjectId = "Object1", Value = "VALUE1"});
                Console.WriteLine("Press any key to read object the server...");
                Console.ReadKey();
                Console.WriteLine("Value received: " + ServerService.read(new ReadRequest { PartitionId = "partition1", ObjectId = "Object1" }).Value);
            }
            else if (id == "server2")
            {
                Console.WriteLine("Press any key to partition the server...");
                Console.ReadKey();
                ServerService.partition(new PartitionRequest { Name = "partition2", Ids = { "server2", "server3" } });
                Console.WriteLine("Press any key to read object the server...");
                Console.ReadKey();
                Console.WriteLine("Value received: " + ServerService.read(new ReadRequest { PartitionId = "partition1", ObjectId = "Object1" }).Value);
            }
            else
            {
                Console.WriteLine("Press any key to partition the server...");
                Console.ReadKey();
                ServerService.partition(new PartitionRequest { Name = "partition3", Ids = { "server3", "server1" } });
            }
            Console.WriteLine("Press any key to status the server...");
            Console.ReadKey();
            ServerService.status(new StatusRequest());
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
    }
}
