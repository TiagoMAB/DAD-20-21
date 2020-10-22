using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using Client.Exceptions;
using System.Linq;

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

            //Trying current connected server
            if (partitions != null && partitions.Contains(this.partitionId))
            {
                try
                {
                    channel = GrpcChannel.ForAddress(serverInfo.CurrentServerURL);
                    client = new GStore.GStore.GStoreClient(channel);
                    response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                    Console.WriteLine(response.Value);
                    return;
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable) {
                    if (String.Equals(this.serverId, "-1")) {
                        System.Diagnostics.Debug.WriteLine(String.Format("Server with URL {0} not available, trying random server.", serverInfo.CurrentServerURL));
                    }
                    else System.Diagnostics.Debug.WriteLine(String.Format("Server with URL {0} not available, trying server with id {1}.", serverInfo.CurrentServerURL, this.serverId));
                }
            }

            //Trying server with id "serverId"
            if (!String.Equals(this.serverId, "-1"))
            {
                string nextURL = serverInfo.GetURLByServerId(this.serverId);
                partitions = serverInfo.GetPartitionsByURL(serverInfo.CurrentServerURL);

                if (nextURL == null)
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL {0} doesn't exist, trying random server.", nextURL));

                else if (partitions != null && partitions.Contains(this.partitionId))
                {
                    try
                    {
                        serverInfo.CurrentServerURL = nextURL;

                        channel = GrpcChannel.ForAddress(nextURL);
                        client = new GStore.GStore.GStoreClient(channel);
                        response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                        Console.WriteLine(response.Value);
                        return;
                    }
                    catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Server with URL {0} not available, trying random server.", nextURL));
                    }
                }
            }

            //Trying random server with partitionId
            List<string> serversWithPartition = serverInfo.GetURLsWithPartitionId(this.partitionId);
            Random random = new Random();
            string url;

            while (serversWithPartition.Count != 0)
            {
                int i = random.Next(serversWithPartition.Count);
                url = serversWithPartition[i];

                serverInfo.CurrentServerURL = url;
                try
                {
                    channel = GrpcChannel.ForAddress(url);
                    client = new GStore.GStore.GStoreClient(channel);
                    response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                    Console.WriteLine(response.Value);
                    return;
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL {0} not available, trying random server.", url));
                }

                serversWithPartition.RemoveAt(i);
            }

            throw new NonExistentServerException(String.Format("No server with partition id: {0} was found.", this.partitionId));
        }
    }
}
