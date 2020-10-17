using System;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using Client.Exceptions;

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
            ServerInfo serverInfo = ServerInfo.Instance();

            System.Diagnostics.Debug.WriteLine(String.Format("List objects stored in server {0}", this.serverId));

            string url = serverInfo.GetURLByServerId(this.serverId);
            if (url == null)
                throw new NonExistentServerException(String.Format("Server with id {0} not found.", this.serverId));

            var channel = GrpcChannel.ForAddress(url);
            var client = new GStore.GStore.GStoreClient(channel);

            try
            {
                ListServerReply response = client.listServer(new ListServerRequest { });

                Console.WriteLine("  Is Master?\t\tValue");

                foreach (ListServerReply.Types.ListValue value in response.Values)
                    Console.WriteLine("  {1}\t\t{2}", (value.IsMaster)? "true":"false", value.Value);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable) {
                Console.WriteLine("Server with id {0} not available. Exiting...");
                throw;
            }
        }
    }
}
