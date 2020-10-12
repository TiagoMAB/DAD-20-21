using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Linq;

namespace Client
{
    sealed class ServerInfo
    {
        static ServerInfo serverInfo = null;

        readonly Dictionary<string, string> serverURL = new Dictionary<string, string>(); // <serverId, URL>

        readonly Dictionary<string, string> masterURL = new Dictionary<string, string>(); // <partitionId, URL>

        //TODO: maybe not needed
        readonly Dictionary<string, List<string>> serverReplicas = new Dictionary<string, List<string>>(); // <URL, partitionIds>

        public string CurrentServerURL { get; set; }
        public bool ExecFinish { get; set; }
        public string UserName { get; set; }

        public static ServerInfo Instance()
        {
            if (serverInfo == null)
                return (serverInfo = new ServerInfo());
            else 
                return serverInfo;
        }

        public void AddServerURL(string serverId, string url)
        {
            serverURL.Add(serverId, url);
        }

        public void RegisterServer(string serverId, string url, List<string> masterPartitionId, List<string> partitions) {
            serverURL.Add(serverId, url);

            if (masterPartitionId.Count != 0)
            {
                foreach (string part in masterPartitionId)
                    masterURL.Add(part, url);
            }

            serverReplicas.Add(url, partitions);
        }

        public void GetServerInfo()
        {
            //Ask other servers' details
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(CurrentServerURL);
            var client = new GStore.GStore.GStoreClient(channel);

            var response = client.serverInfo( new GStore.ServerInfoRequest {} );

            foreach (var value in response.Servers) {
                RegisterServer(value.ServerId, value.Url, value.MasterPartitionId.ToList(), value.Partitions.ToList());
            }
        }
    }
}
