using System;
using GStore;
using Grpc.Core;
using Grpc.Net.Client;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4 && args.Length != 6) 
            {
                Console.WriteLine("Wrong format. Should be: id url min_delay max_delay [otherId otherURL]\n otherId and otherURL are optional arguments used to connect to a pre-existing network.\nPress any key to exit...");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            string id = args[0];
            string URL = args[1];
            string otherId = null, otherURL = null; //TO DO: Remove and insert in else
            int min_delay = int.Parse(args[2]);
            int max_delay = int.Parse(args[3]);
            Random r = new Random();
            int delay = r.Next(min_delay, max_delay);

            string[] details = URL.Split("//");
            details = details[1].Split(':');

            string host = details[0];
            int port = int.Parse(details[1]);

            ServerService ServerService;

            if (args.Length == 4) 
            {
                ServerService = new ServerService(id, URL, delay);
            }
            else
            {
                otherId = args[4];
                otherURL = args[5];
                ServerService = new ServerService(id, URL, delay, otherId, otherURL);
            }

            GStoreService gstoreservice = new GStoreService(ServerService);
            PuppetMasterService puppetmasterservice = new PuppetMasterService(ServerService);
            ServerCommunicationService servercommunicationservice = new ServerCommunicationService(ServerService);

            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = 
                { 
                    GStore.GStore.BindService(gstoreservice), 
                    PuppetMaster.BindService(puppetmasterservice), 
                    ServerCommunication.BindService(servercommunicationservice) 
                },
                Ports = 
                { 
                    new ServerPort(host, port, ServerCredentials.Insecure) 
                }
            };

            server.Start();

            Console.WriteLine("GStore server running on " + host + " listening on port " + port + "");

            //TO DO: Remove in final version
            //debug(id, URL, host, port, otherURL);

            ConsoleKeyInfo k;
            do
            {
                Console.WriteLine("Press 'p' if you want to create a new partition (current server will be the master).\nPress 's' to print the server status.\nPress 'e' to stop the server.");
                k = Console.ReadKey();

                switch (k.KeyChar)
                {
                    case 'p':
                        Console.WriteLine("\nWrite the name of the partition.");
                        string name = Console.ReadLine();

                        Console.WriteLine("Write the ids of replicas belonging to the partition, separated by a comma. The first one must be the current server id.\nExample: server1,server2,server3");
                        string line = Console.ReadLine();

                    
                        string[] ids = line.Split(',');
                        PartitionRequest request = new PartitionRequest { Name = name};
                        request.Ids.AddRange(ids);
                        ServerService.partition(request);
                        break;

                    case 's':
                        Console.WriteLine();
                        ServerService.status(new StatusRequest());
                        break;

                    case 'e':
                        break;

                    default:
                        Console.WriteLine("Command not recognized.");
                        break;
                }

            } while (k.KeyChar != 'e');

            server.ShutdownAsync().Wait();
        }

        //TO DO: Remove in final version
        static void debug(string id, string URL, string host, int port, string otherURL)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(URL);
            PuppetMaster.PuppetMasterClient client = new PuppetMaster.PuppetMasterClient(channel);
            GStore.GStore.GStoreClient client1 = new GStore.GStore.GStoreClient(channel);

            client.Status(new StatusRequest());
            if (otherURL == null)
            {
                Console.WriteLine("Press any key to partition the server...");
                Console.ReadKey();
                client.Partition(new PartitionRequest { Name = "partition1", Ids = { "server1", "server2" } });
                Console.WriteLine("Press any key to write object the server...");
                Console.ReadKey();
                client1.Write(new WriteRequest { PartitionId = "partition1", ObjectId = "Object1", Value = "VALUE1" });
                Console.WriteLine("Press any key to read object the server...");
                Console.ReadKey();
                Console.WriteLine("Value received: " + client1.Read(new ReadRequest { PartitionId = "partition1", ObjectId = "Object1" }).Value);
            }
            else if (id == "server2")
            {
                Console.WriteLine("Press any key to partition the server...");
                Console.ReadKey();
                client.Partition(new PartitionRequest { Name = "partition2", Ids = { "server2", "server3" } });
                Console.WriteLine("Press any key to read object the server...");
                Console.ReadKey();
                Console.WriteLine("Value received: " + client1.Read(new ReadRequest { PartitionId = "partition1", ObjectId = "Object1" }).Value);
                Console.WriteLine("Press any key to crash the server...");
                Console.ReadKey();
                client.Crash(new CrashRequest());
            }
            else
            {
                Console.WriteLine("Press any key to partition the server...");
                Console.ReadKey();
                client.Partition(new PartitionRequest { Name = "partition3", Ids = { "server3", "server1" } });
            }
            Console.WriteLine("Press any key to status the server...");
            Console.ReadKey();
            client.Status(new StatusRequest());
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
        }
    }
}
