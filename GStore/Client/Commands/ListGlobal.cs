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
            string message = "";
            List<string> urls = serverInfo.GetURLs();

            Console.WriteLine("List Global: Printing all servers:");

            foreach (string url in urls)
            {
                try
                {
                    GStore.GStore.GStoreClient client = serverInfo.GetChannel(url);

                    Console.WriteLine("\tList server: Printing values of \"{0}\":", url);

                    ListServerReply response = client.ListServer(new ListServerRequest { });

                    foreach (ListServerReply.Types.ListValue value in response.Values)
                        message += String.Format("\t\tPartition Id: {0}\n" +
                            "\t\tObject Id: {1}\n" +
                            "\t\tValue: {2}\n" +
                            "\t\tIs this server the master of the object? {3}\n",
                            value.PartitionId, value.ObjectId, value.Value, (value.IsMaster)? "true":"false");
                }
                catch (RpcException e)
                {
                    Console.WriteLine("\tServer with URL \"{0}\" failed with status \"{1}\". Proceeding to next operation...", url, e.StatusCode.ToString());
                }
            }

            serverInfo.CurrentServerURL = currentServerURL;

            Console.WriteLine(message);
            Console.WriteLine("All values printed.\n\n");
        }
    }
}
