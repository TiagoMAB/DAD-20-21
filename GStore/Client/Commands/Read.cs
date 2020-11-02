using System;
using System.Collections.Generic;
using Grpc.Core;
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

                    Console.WriteLine("Contacted server with URL \"{0}\"\nThe requested value: {1}\n\n", serverInfo.CurrentServerURL, response.Value);
                    return;
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
                partitions = serverInfo.GetPartitionsByURL(nextURL);

                if (nextURL == null)
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" doesn't exist, trying random server.", nextURL));

                else if (partitions != null && partitions.Contains(this.partitionId))
                {
                    try
                    {
                        client = serverInfo.GetChannel(nextURL);
                        response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                        Console.WriteLine("Contacted server with URL \"{0}\"\nThe requested value: {1}\n\n", serverInfo.CurrentServerURL, response.Value);
                        return;
                    }
                    catch (RpcException e)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\", trying random server.", nextURL, e.StatusCode.ToString()));
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
                try
                {
                    client = serverInfo.GetChannel(url);
                    response = client.Read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId });

                    Console.WriteLine("Contacted server with URL \"{0}\"\nThe requested value: {1}\n\n", serverInfo.CurrentServerURL, response.Value);
                    return;
                }
                catch (RpcException e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\", trying random server.", url, e.StatusCode.ToString()));
                }

                serversWithPartition.RemoveAt(i);
            }

            throw new NonExistentServerException(String.Format("No server with partition id \"{0}\" was found.", this.partitionId));
        }
    }
}
