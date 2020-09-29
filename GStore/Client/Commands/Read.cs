using System;

namespace Client.Commands
{
    class Read : Command
    {
        private readonly string partitionId;
        private readonly string objectId;
        private readonly string serverId;

        public Read(string partitionId, string objectId, string serverId)
        {
            this.partitionId = partitionId;
            this.objectId = objectId;
            this.serverId = serverId;
        }

        public void Execute()
        {
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("Read in partition {0} for object {1} with optional fetch server {2}", this.partitionId, this.objectId, this.serverId));
        }
    }
}
