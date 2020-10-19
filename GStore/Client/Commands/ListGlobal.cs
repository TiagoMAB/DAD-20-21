using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using Client.Exceptions;
using System.Linq;

namespace Client.Commands
{
    class ListGlobal : Command
    {
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("Listing the partition and object identifiers of all objects stored on the system.");

            ServerInfo serverInfo = ServerInfo.Instance();
            List<string> partitionsToRequest = serverInfo.GetPartitionIds();
            List<string> urls = serverInfo.GetURLs();
            Random random = new Random();
            List<ListGlobalReply> replies = new List<ListGlobalReply>();

            //Picks random server to ask for partition's information
            while (partitionsToRequest.Count != 0 && urls.Count != 0)
            {
                string url = urls[random.Next(urls.Count)];

                List<string> serverPartitionIds = serverInfo.GetPartitionsByURL(url);
                ListGlobalRequest listGlobal = new ListGlobalRequest();

                foreach (string partitionId in serverPartitionIds)
                    if (partitionsToRequest.Exists(request => String.Equals(request, partitionId)))
                        listGlobal.PartitionIds.Add(partitionId);

                if (listGlobal.PartitionIds.Count == 0)
                {
                    urls.Remove(url);
                    continue;
                }

                serverInfo.CurrentServerURL = url;

                var channel = GrpcChannel.ForAddress(serverInfo.CurrentServerURL);
                var client = new GStore.GStore.GStoreClient(channel);
                try
                {
                    //TODO: Timeout can be added, but should we do it?
                    replies.Add(client.ListGlobal(listGlobal));

                    urls.Remove(url);
                    foreach (string partition in serverPartitionIds)
                        partitionsToRequest.Remove(partition);
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL {0} not available. Asking next server.", serverInfo.CurrentServerURL));
                    urls.Remove(url);
                }
            }
            if (urls.Count == 0 && partitionsToRequest.Count != 0)
                throw new NonExistentServerException("No more servers available to contact.");

            Console.WriteLine("Partition id:\t\t\tObject id:");
            foreach (ListGlobalReply listPartition in replies)
                foreach (ListGlobalReply.Types.ListPartition partition in listPartition.Partitions.ToList())
                    foreach (string id in partition.ObjectId.ToList())
                        Console.WriteLine("{0}\t\t\t{1}", partition.PartitionId, id);
        }
    }
}
