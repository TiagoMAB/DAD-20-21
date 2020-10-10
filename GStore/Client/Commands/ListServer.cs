using System;
using Grpc.Net.Client;
using GStore;

namespace Client.Commands
{
    class ListServer : Command
    {
        private readonly string serverId;

        public ListServer(string serverId)
        {
            this.serverId = serverId;
        }

        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("List objects stored in server {0}", this.serverId));

            var channel = GrpcChannel.ForAddress("https://localhost:5001"); //server ports?
            var client = new GStore.GStore.GStoreClient(channel);

            var response = client.listServer(new ListServerRequest {} );

            Console.WriteLine("  Is Master?\tValue");

            foreach (var value in response.Values)
                Console.WriteLine("  {1}\t{2}", (value.IsMaster)? "true":"false", value.Value);
        }
    }
}
