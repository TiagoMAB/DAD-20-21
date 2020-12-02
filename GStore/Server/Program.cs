using System;
using GStore;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private const int GOSSIP_DELAY = 20;

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
                string otherId = args[4];
                string otherURL = args[5];
                ServerService = new ServerService(id, URL, delay, otherId, otherURL);
            }

            new System.Threading.Timer((e) => ServerService.gossip(), null, 0, GOSSIP_DELAY * 1000);

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

            ConsoleKeyInfo k;
            do
            {
                Console.WriteLine("Press 'p' if you want to create a new partition.\nPress 'f' freeze the server.\nPress 'u' to unfreeze the server status.\nPress 's' to print the server status.\nPress 'c' to crash the server.\nPress 'w' to write an object.\nPress 'r' to read an object.\nPress 'e' to stop the server.");
                k = Console.ReadKey();

                switch (k.KeyChar)
                {
                    case 'f':
                        Console.WriteLine();
                        ServerService.freeze(new FreezeRequest());
                        break;

                    case 'u':
                        Console.WriteLine();
                        ServerService.unfreeze(new UnfreezeRequest());
                        break;

                    case 'c':
                        Console.WriteLine();
                        ServerService.crash(new CrashRequest());
                        break;
                    
                    case 'p':
                        Console.WriteLine("\nWrite the name of the partition.");
                        string name = Console.ReadLine();

                        Console.WriteLine("Write the ids of replicas belonging to the partition, separated by a comma.\nExample: server1,server2,server3");
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

                    case 'w':
                        Console.WriteLine("\nWrite the partition id.");
                        string partition_id = Console.ReadLine();

                        Console.WriteLine("\nWrite the object id.");
                        string object_id = Console.ReadLine();

                        Console.WriteLine("\nWrite the object value.");
                        string value = Console.ReadLine();

                        Task t = ServerService.write(new WriteRequest { PartitionId = partition_id, ObjectId = object_id, Value = value});
                        break;

                    case 'r':
                        Console.WriteLine("\nWrite the partition id.");
                        string read_partition_id = Console.ReadLine();

                        Console.WriteLine("\nWrite the object id.");
                        string read_object_id = Console.ReadLine();

                        ServerService.read(new ReadRequest { PartitionId = read_partition_id, ObjectId = read_object_id });
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
    }
}
