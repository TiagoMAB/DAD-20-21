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

            System.Diagnostics.Debug.WriteLine(String.Format("List objects stored in server \"{0}\"", this.serverId));

            string url = serverInfo.GetURLByServerId(this.serverId);
            if (url == null) {
                Console.WriteLine("Server with id \"{0}\" not found.", this.serverId);
                return;
            }

            GStore.GStore.GStoreClient client = serverInfo.GetChannel(url);
            try
            {
                Console.WriteLine("List server: Printing values of \"{0}\":", this.serverId);

                ListServerReply response = client.ListServer(new ListServerRequest { });

                foreach (ListServerReply.Types.ListValue value in response.Values)
                    Console.WriteLine("\tPartition Id: {0}\n" +
                        "\tObject Id: {1}\n" +
                        "\tValue: {2}\n" +
                        "\tIs this server the master of the object? {3}\n",
                        value.PartitionId, value.ObjectId, value.Value, (value.IsMaster)? "true":"false");

                Console.WriteLine("All values printed.\n\n");
            }
            catch (RpcException e)
            {
                Console.WriteLine("Server with id \"{0}\" failed with status \"{1}\". Proceeding to next operation...", this.serverId, e.StatusCode.ToString());
            }
        }
    }
}
