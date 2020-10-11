using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;

namespace Server
{
    public class GStoreService : GStore.GStore.GStoreBase
    {

        public GStoreService(int id, string URL, string otherURL)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(URL);
            var server = new GStore.GStore.GStoreClient(channel);
        }

        public override Task<GStore.HandshakeReply> handshake(HandshakeRequest request, ServerCallContext context)
        {
            Console.WriteLine("Handshake");
            return Task.FromResult(new HandshakeReply());
        }


        public override Task<GStore.WriteReply> write(WriteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Write");
            return Task.FromResult(new WriteReply());
        }

        public override Task<GStore.ReadReply> read(ReadRequest request, ServerCallContext context)
        {
            Console.WriteLine("Read");
            return Task.FromResult(new ReadReply());
        }
    }
}