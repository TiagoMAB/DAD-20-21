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

        public override Task<LockObjectReply> LockObject(LockObjectRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.lockObject(request));
        }

        public override Task<WriteObjectReply> WriteObject(WriteObjectRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.writeObject(request));
        }

        public override Task<SharePartitionReply> SharePartition(SharePartitionRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.sharePartition(request));
        }
    }
}