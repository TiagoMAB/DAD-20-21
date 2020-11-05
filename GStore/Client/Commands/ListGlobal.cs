using System;
using System.Collections.Generic;
using Grpc.Core;
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
            string currentServerURL = serverInfo.CurrentServerURL;
            List<string> partitionsToRequest = serverInfo.GetPartitionIds();
            List<string> urls = serverInfo.GetURLs();
            Random random = new Random();
            List<ListGlobalReply> replies = new List<ListGlobalReply>();

            Console.WriteLine("List Global: Printing all partition and object Ids:");

            //Picks random server to ask for partition's information
            while (partitionsToRequest.Count != 0 && urls.Count != 0)
            {
                string url = urls[random.Next(urls.Count)];

                List<string> serverPartitionIds = serverInfo.GetPartitionsByURL(url);
                ListGlobalRequest listGlobal = new ListGlobalRequest();

                if (serverPartitionIds == null)
                {
                    urls.Remove(url);
                    continue;
                }

                foreach (string partitionId in serverPartitionIds)
                    if (partitionsToRequest.Contains(partitionId))
                        listGlobal.PartitionIds.Add(partitionId);

                if (listGlobal.PartitionIds.Count == 0)
                {
                    urls.Remove(url);
                    continue;
                }

                try
                {
                    GStore.GStore.GStoreClient client = serverInfo.GetChannel(url);

                    replies.Add(client.ListGlobal(listGlobal));

                    urls.Remove(url);
                    foreach (string partition in serverPartitionIds)
                        partitionsToRequest.Remove(partition);
                }
                catch (RpcException e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\". Asking next server.", serverInfo.CurrentServerURL, e.StatusCode.ToString()));
                    urls.Remove(url);
                }
            }
            if (urls.Count == 0 && partitionsToRequest.Count != 0)
                throw new NonExistentServerException("No more servers available to contact.");

            serverInfo.CurrentServerURL = currentServerURL;

            Console.WriteLine("Partition id:\t\t\tObject id:");
            foreach (ListGlobalReply listPartition in replies)
                foreach (ListGlobalReply.Types.ListPartition partition in listPartition.Partitions.ToList())
                    foreach (string id in partition.ObjectId.ToList())
                        Console.WriteLine("{0}\t\t\t\t{1}", partition.PartitionId, id);
            Console.WriteLine("All values printed.\n\n");
        }
    }
}
