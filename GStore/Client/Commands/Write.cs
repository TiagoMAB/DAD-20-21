using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Commands
{
    class Write : Command
    {
        private readonly string partitionId;
        private readonly string objectId;
        private readonly string value;

        public Write(string partitionId, string objectId, string value)
        {
            this.partitionId = partitionId;
            this.objectId = objectId;
            this.value = value;
        }

        public void Execute()
        {
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("Write in partition {0} with object id {1} and value {2}", this.partitionId, this.objectId, this.value));
        }
    }
}
