using System;
using Grpc.Net.Client;
using GStore;

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
            System.Diagnostics.Debug.WriteLine(String.Format("Read in partition {0} for object {1} with optional fetch server {2}", this.partitionId, this.objectId, this.serverId));

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress("https://localhost:5001"); //server ports?
            var client = new GStore.GStore.GStoreClient(channel);

            var response = client.read(new ReadRequest { PartitionId = this.partitionId, ObjectId = this.objectId } );

            Console.WriteLine(response.Value);
        }
    }
}
