using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    sealed class ServerInfo
    {
        static ServerInfo serverInfo = null;
        public string CurrentHost { get; set; }
        public int CurrentPort { get; set; }

        public bool ExecFinish { get; set; }

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
