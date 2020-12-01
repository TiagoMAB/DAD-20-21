using System.Threading.Tasks;
using Grpc.Core;
using GStore;

namespace Server
{
    public class ServerCommunicationService : ServerCommunication.ServerCommunicationBase
    {

        private ServerService server;

        public ServerCommunicationService(ServerService server)
        {
            this.server = server;
        }

        public override Task<HandshakeReply> Handshake(HandshakeRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.handshake(request));
        }

        public override Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.register(request));
        }

        public override Task<SharePartitionReply> SharePartition(SharePartitionRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.sharePartition(request));
        }

        public override Task<GossipReply> Gossip(GossipRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.gossip(request));
        }

        public override Task<GetUniqueIdReply> GetUniqueId(GetUniqueIdRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.getUniqueId(request));
        }

        public override Task<GetDelayReply> GetDelay(GetDelayRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.getDelay(request));
        }
    }
}