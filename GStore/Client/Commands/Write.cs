using System;
using Grpc.Core;
using Grpc.Net.Client;
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

            System.Diagnostics.Debug.WriteLine(String.Format("Write in partition {0} with object id {1} and value {2}", this.partitionId, this.objectId, this.value));

            string masterURL = serverInfo.GetMasterURLByPartitionId(this.partitionId);
            if (masterURL == null)
                throw new NonExistentServerException(String.Format("Master server of partition {0} not found.", this.partitionId));

            if (!String.Equals(serverInfo.CurrentServerURL, masterURL))
                serverInfo.CurrentServerURL = masterURL;

            var channel = GrpcChannel.ForAddress(serverInfo.CurrentServerURL);
            var client = new GStore.GStore.GStoreClient(channel);

            try
            {
                //TODO: a chamada deve ser async no codigo base?
                client.write(new WriteRequest { PartitionId = this.partitionId, ObjectId = this.objectId, Value = this.value });
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Master server with URL {0} not available. Exiting...", serverInfo.CurrentServerURL));
                throw;
            }

        }
    }
}
