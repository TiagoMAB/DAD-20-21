using System;
using Grpc.Core;
using GStore;
using System.Linq;

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
            string currentServerURL = serverInfo.CurrentServerURL;

            System.Diagnostics.Debug.WriteLine(String.Format("List objects stored in server \"{0}\"", this.serverId));

            string url = null;

            foreach (int retries in Enumerable.Range(1, serverInfo.numOfRetries).Reverse())
            {
                url = serverInfo.GetURLByServerId(this.serverId);
                if (url == null)
                {
                    Console.WriteLine("Server with id \"{0}\" not found.", this.serverId);
                    System.Threading.Thread.Sleep(serverInfo.backoffTime);
                    serverInfo.GetServerInfo();
                }
                else break;
            }

            if (url == null)
            {
                Console.WriteLine("No more retries will be done. Proceeding...\n");
                return;
            }

            try
            {
                GStore.GStore.GStoreClient client = serverInfo.GetChannel(url);

                Console.WriteLine("List server: Printing values of \"{0}\":", this.serverId);

                ListServerReply response = client.ListServer(new ListServerRequest { });

                foreach (ListServerReply.Types.ListValue value in response.Values)
                    Console.WriteLine("\tPartition Id: {0}\n" +
                        "\tObject Id: {1}\n" +
                        "\tValue: {2}\n" +
                        "\tIs this server the master of the object? yes\n",
                        value.PartitionId, value.ObjectId, value.Value);

                Console.WriteLine("All values printed.\n\n");

                foreach (ListServerReply.Types.Timestamps part in response.PartTimestamp)
                    serverInfo.updatePartitionTimestamp(part.PartitionId, part.Timestamp);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Server with id \"{0}\" failed with status \"{1}\". Proceeding to next operation...", this.serverId, e.StatusCode.ToString());
            }

            if (serverInfo.GetURLs().Contains(currentServerURL))
                serverInfo.CurrentServerURL = currentServerURL;
        }
    }
}
