using System.Threading.Tasks;
using Grpc.Core;
using GStore;

namespace Server
{
    public class GStoreService : GStore.GStore.GStoreBase
    {

        private ServerService server;
            
        public GStoreService(ServerService server)
        {
            this.server = server;
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.write(request));
        }

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.read(request));
        }

        public override Task<ServerInfoReply> ServerInfo(ServerInfoRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.serverInfo(request));
        }

        public override Task<ListServerReply> ListServer(ListServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.listServer(request));
        }
    }
}