using System;
using Grpc.Core;
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

            GStore.GStore.GStoreClient client = serverInfo.GetChannel(url);

            try
            {
                ListServerReply response = client.ListServer(new ListServerRequest { });

                foreach (ListServerReply.Types.ListValue value in response.Values)
                    Console.WriteLine("---------------------//---------------------\n" +
                        "Partition Id: {0}\n" +
                        "Object Id: {1}\n" +
                        "Value: {2}\n" +
                        "Is this server the master of the object? {3}",
                        value.PartitionId, value.ObjectId, value.Value, (value.IsMaster)? "true":"false");
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable) {
                Console.WriteLine("Server with id {0} not available. Exiting...");
                throw;
            }
        }
    }
}
