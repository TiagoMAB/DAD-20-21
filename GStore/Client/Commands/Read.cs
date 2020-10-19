using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using Client.Exceptions;

namespace Client.Commands
{
    class Read : Command
    {
        private readonly string partitionId;
        private readonly string objectId;
        private readonly string serverId;

        public Read(string partitionId, string objectId, string serverId)
        {
            this.partitionId = partitionId;
            this.objectId = objectId;
            this.serverId = serverId;
        }

        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Read in partition {0} for object {1} with optional fetch server {2}", this.partitionId, this.objectId, this.serverId));
            ServerInfo serverInfo = ServerInfo.Instance();
            GrpcChannel channel;
            GStore.GStore.GStoreClient client;
            ReadReply response;

            List<string> partitions = serverInfo.GetPartitionsByURL(serverInfo.CurrentServerURL);

            if (partitions != null && partitions.Contains(this.partitionId))
            {
                try
                {
                    channel = GrpcChannel.ForAddress(serverInfo.CurrentServerURL);
                    client = new GStore.GStore.GStoreClient(channel);
                    response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                    Console.WriteLine(response.Value);
                    //TODO: é suposto o server retornar N/A e sendo esse o caso é necessário procurar no secundário?
                    //TODO: talvez recebe se um null em vez de N/A do server, e sendo esse o caso falta meter um if no writeline
                    return;
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable) {
                    if (String.Equals(this.serverId, "-1")) {
                        Console.WriteLine("Secondary server isn't given. Exiting...");
                        throw; //ignore secondary server
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL {0} not available, trying server with id {1}.", serverInfo.CurrentServerURL, this.serverId));
                }
            }

            string nextURL = serverInfo.GetURLByServerId(this.serverId);
            if (nextURL == null)
                throw new NonExistentServerException(String.Format("Server with id {0} not found.", this.serverId));
            //TODO: necessary to check if nextURL has partition?

            try
            {
                serverInfo.CurrentServerURL = nextURL;

                channel = GrpcChannel.ForAddress(nextURL);
                client = new GStore.GStore.GStoreClient(channel);
                response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                Console.WriteLine(response.Value);
                return;
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable) {
                Console.WriteLine("Secondary server not available. Exiting...");
                throw;
            }
        }
    }
}
