using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using Client.Exceptions;

namespace Client.Commands
{
    class ListGlobal : Command
    {
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("Listing the partition and object identifiers of all objects stored on the system.");

            ServerInfo serverInfo = ServerInfo.Instance();
            List<string> requestedReplicas = serverInfo.GetPartitionIds();


        }
    }
}
