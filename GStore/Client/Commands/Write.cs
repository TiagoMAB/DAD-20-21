using System;
using Grpc.Core;
using GStore;
using System.Collections.Generic;
using System.Linq;

namespace Client.Commands
{
    class Write : Command
    {
        private readonly string partitionId;
        private readonly string objectId;
        private readonly string value;

        public Write(string partitionId, string objectId, string value)
        {
            this.partitionId = partitionId;
            this.objectId = objectId;
            this.value = value;
        }

        public void Execute()
        {
            Random random = new Random();
            ServerInfo serverInfo = ServerInfo.Instance();

            Console.WriteLine("Write in partition \"{0}\" with object id \"{1}\" and value \"{2}\"...", this.partitionId, this.objectId, this.value);

            List<string> urls = null;

            foreach (int retries in Enumerable.Range(1, serverInfo.numOfRetries).Reverse())
            {
                urls = serverInfo.GetURLsWithPartitionId(this.partitionId);
                if (urls.Count == 0)
                {
                    Console.WriteLine("Server with partitionId \"{0}\" not found. Retrying {1} more time(s) in {2}ms...\n", this.partitionId, retries, serverInfo.backoffTime);
                    System.Threading.Thread.Sleep(serverInfo.backoffTime);
                    serverInfo.GetServerInfo();
                }
                else break;
            }

            if (urls == null || urls.Count == 0)
            {
                Console.WriteLine("No more retries will be done. Proceeding...\n");
                return;
            }

            //TODO: In case uniqueId is reused
            //string uniqueId = serverInfo.UniqueId;

            foreach (string url in urls.OrderBy(randomURL => random.Next()))
            {
                try
                {
                    GStore.GStore.GStoreClient client = serverInfo.GetChannel(url);
                    WriteRequest request = new WriteRequest { PartitionId = this.partitionId, ObjectId = this.objectId, Value = this.value /*, UniqueId = uniqueId*/ };

                    WriteReply reply = client.Write(request);

                    Console.WriteLine("Write of value \"{0}\" completed.\n", this.value);

                    serverInfo.updatePartitionTimestamp(this.partitionId, reply.Timestamp);
                    return;
                }
                catch (RpcException e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Master server with URL \"{0}\" failed with status \"{1}\".", serverInfo.CurrentServerURL, e.StatusCode.ToString()));
                    Console.WriteLine("Master server with URL \"{0}\" failed with status \"{1}\". Retrying write...", serverInfo.CurrentServerURL, e.StatusCode.ToString());
                }
            }
            Console.WriteLine("No more retries will be done.Proceeding...\n");
        }
    }
}
