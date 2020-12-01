using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using GStore;

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
            System.Diagnostics.Debug.WriteLine(String.Format("Read in partition \"{0}\" for object \"{1}\" with optional fetch server \"{2}\"", this.partitionId, this.objectId, this.serverId));
            ServerInfo serverInfo = ServerInfo.Instance();
            GStore.GStore.GStoreClient client;
            ReadReply response;

            List<string> partitions = serverInfo.GetPartitionsByURL(serverInfo.CurrentServerURL);

            Console.WriteLine("Reading value in partition \"{0}\" for object \"{1}\" and alternative server \"{2}\":", this.partitionId, this.objectId, this.serverId);

            //Trying current connected server
            if (partitions != null && partitions.Contains(this.partitionId))
            {
                try
                {
                    client = serverInfo.GetChannel(serverInfo.CurrentServerURL);
                    response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                    if (serverInfo.IsNewerTimestamp(this.partitionId, response.Timestamp))
                    {
                        Console.WriteLine("Contacted server with URL \"{0}\"\nThe requested value: {1}\n\n", serverInfo.CurrentServerURL, response.Value);
                        serverInfo.updatePartitionTimestamp(this.partitionId, response.Timestamp);
                        return;
                    }
                }
                catch (RpcException e) {
                    if (String.Equals(this.serverId, "-1")) {
                        System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\", trying random server.", serverInfo.CurrentServerURL, e.StatusCode.ToString()));
                    }
                    else System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\", trying server with id {2}.", serverInfo.CurrentServerURL, e.StatusCode.ToString(), this.serverId));
                }
            }

            //Trying server with id "serverId"
            if (!String.Equals(this.serverId, "-1"))
            {
                string nextURL = serverInfo.GetURLByServerId(this.serverId);

                if (nextURL == null)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" doesn't exist, updating network information...", nextURL));
                    serverInfo.GetServerInfo();
                    nextURL = serverInfo.GetURLByServerId(this.serverId);
                }

                partitions = serverInfo.GetPartitionsByURL(nextURL);

                if (partitions != null && partitions.Contains(this.partitionId))
                {
                    try
                    {
                        client = serverInfo.GetChannel(nextURL);
                        response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                        if (serverInfo.IsNewerTimestamp(this.partitionId, response.Timestamp))
                        {
                            Console.WriteLine("Contacted server with URL \"{0}\"\nThe requested value: {1}\n\n", serverInfo.CurrentServerURL, response.Value);
                            serverInfo.updatePartitionTimestamp(this.partitionId, response.Timestamp);
                            return;
                        }
                    }
                    catch (RpcException e)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\", trying random server.", nextURL, e.StatusCode.ToString()));
                    }
                }
                else Console.WriteLine("Server with id {0} doesn't have requested partition or it's unnavailable. Trying random server...", this.serverId);
            }

            //Trying random server with partitionId
            List<string> serversWithPartition = null;
            Random random = new Random();

            foreach (int retries in Enumerable.Range(1, serverInfo.numOfRetries).Reverse())
            {
                serversWithPartition = serverInfo.GetURLsWithPartitionId(this.partitionId);
                if (serversWithPartition.Count == 0)
                {
                    Console.WriteLine("Server with partitionId \"{0}\" not found. Retrying {1} more time(s) in {2}ms...\n", this.partitionId, retries, serverInfo.backoffTime);
                    System.Threading.Thread.Sleep(serverInfo.backoffTime);
                    serverInfo.GetServerInfo();
                }
                else break;
            }

            if (serversWithPartition == null || serversWithPartition.Count == 0)
            {
                Console.WriteLine("No server with partition id \"{0}\" was found.", this.partitionId);
                return;
            }

            foreach (string url in serversWithPartition.OrderBy(randomURL => random.Next()))
            {
                try
                {
                    client = serverInfo.GetChannel(url);
                    response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                    if (serverInfo.IsNewerTimestamp(this.partitionId, response.Timestamp))
                    {
                        Console.WriteLine("Contacted server with URL \"{0}\"\nThe requested value: {1}\n\n", serverInfo.CurrentServerURL, response.Value);
                        serverInfo.updatePartitionTimestamp(this.partitionId, response.Timestamp);
                        return;
                    }
                }
                catch (RpcException e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\", trying random server.", url, e.StatusCode.ToString()));
                }
            }

            Console.WriteLine("No server with recent enough timestamp was found.\n\n");
        }
    }
}
