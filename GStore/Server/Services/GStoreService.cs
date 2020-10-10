using System.Threading.Tasks;
using Grpc.Core;
using System;

namespace Server
{
    public class GStoreService : GStore.GStore.GStoreBase
    {

        public GStoreService()
        {
        }

        public override Task<GStore.WriteReply> write(GStore.WriteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Read");
            return Task.FromResult(new GStore.WriteReply());
        }

        public override Task<GStore.ReadReply> read(GStore.ReadRequest request, ServerCallContext context)
        {
            Console.WriteLine("Reply");
            return Task.FromResult(new GStore.ReadReply());
        }
    }
}