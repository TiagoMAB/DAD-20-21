using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    sealed class ServerInfo
    {
        static ServerInfo serverInfo = null;

        readonly Dictionary<string, string> serverURL = new Dictionary<string, string>(); // <serverId, URL>

        readonly Dictionary<string, string> masterURL = new Dictionary<string, string>(); // <partitionId, URL>

        //TODO: maybe not needed
        readonly Dictionary<string, List<string>> serverReplicas = new Dictionary<string, List<string>>(); // <url, partitionIds>

        public string CurrentServerURL { get; set; }
        public bool ExecFinish { get; set; }

        public static ServerInfo Instance()
        {
            if (serverInfo == null)
                return (serverInfo = new ServerInfo());
            else 
                return serverInfo;
        }

        public void AddServer(string serverId, string url, List<string> masterPartitionId, List<string> partitions) {
            serverURL.Add(serverId, url);

            if (masterPartitionId.Count != 0)
            {
                foreach (string part in masterPartitionId)
                    masterURL.Add(part, url);
            }

            serverReplicas.Add(url, partitions);
        }
    }
}
