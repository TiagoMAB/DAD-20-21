using System;
using System.Collections.Generic;
using System.Text;

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
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("List objects stored in server {0}", this.serverId));
        }
    }
}
