using System;
using Grpc.Net.Client;
using GStore;

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

            var channel = GrpcChannel.ForAddress("https://localhost:5001"); //server ports?
            var client = new GStore.GStore.GStoreClient(channel);

            // TODO: Change to the master replica server
            // write calls can be made asynchronously 
            client.write(new WriteRequest { PartitionId = this.partitionId, ObjectId = this.objectId, Value = this.value } );

            Console.WriteLine();
        }
    }
}
