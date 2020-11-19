using System;
using Grpc.Core;
using GStore;
using Client.Exceptions;

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
            ServerInfo serverInfo = ServerInfo.Instance();

            System.Diagnostics.Debug.WriteLine(String.Format("Write in partition \"{0}\" with object id \"{1}\" and value \"{2}\"", this.partitionId, this.objectId, this.value));

            Console.WriteLine("Write in partition \"{0}\" with object id \"{1}\" and value \"{2}\"...", this.partitionId, this.objectId, this.value);

            string masterURL = serverInfo.GetMasterURLByPartitionId(this.partitionId);
            if (masterURL == null)
                throw new NonExistentServerException(String.Format("Master server of partition \"{0}\" not found.", this.partitionId));

            try
            {
                GStore.GStore.GStoreClient client = serverInfo.GetChannel(masterURL);
                client.Write(new WriteRequest { PartitionId = this.partitionId, ObjectId = this.objectId, Value = this.value });
                Console.WriteLine("Write of value \"{0}\" completed.\n", this.value);
            }
            catch (RpcException e)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Master server with URL \"{0}\" failed with status \"{1}\".", serverInfo.CurrentServerURL, e.StatusCode.ToString()));
                Console.WriteLine("Master server with URL \"{0}\" failed with status \"{1}\".", serverInfo.CurrentServerURL, e.StatusCode.ToString());
            }
        }
    }
}
