using Grpc.Core;
using System;

namespace PCS
{
    class Program
    {
        const int PORT = 10000;

        static void Main(string[] args)
        {
            if(args.Length != 2) {
                Console.WriteLine("Usage: PCS.exe <path to client> <path to server>");
                Console.WriteLine("Press any key to shutdown...");
                Console.ReadKey();
                return;
            }

            string clientPath = args[0];
            string serverPath = args[1];

            Server server = new Server
            {
                Services = { GStore.PCS.BindService(new PCSImpl(clientPath, serverPath)) },
                Ports = { new ServerPort("localhost", PORT, ServerCredentials.Insecure) },
            };

            server.Start();

            Console.WriteLine("PCS running on port " + PORT);
            Console.WriteLine("Press any key to shutdown...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
