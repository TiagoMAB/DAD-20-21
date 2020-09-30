using Grpc.Core;
using System;

namespace PCS
{
    class Program
    {
        const int port = 10000;

        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { GStore.PCS.BindService(new PCSImpl()) },
                Ports    = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("PCS running on port " + port);
            Console.WriteLine("Press any key to shutdown...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
