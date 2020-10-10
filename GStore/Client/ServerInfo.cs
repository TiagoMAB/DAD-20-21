using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    sealed class ServerInfo
    {
        static ServerInfo serverInfo = null;
        public string Host { get; set; }
        public int Port { get; set; }

        public static ServerInfo Instance()
        {
            if (serverInfo == null)
                return (serverInfo = new ServerInfo());
            else 
                return serverInfo;
        }

        //TODO: save information about all the servers and their addresses, serverId, replicas here
    }
}
