using System.Threading.Tasks;
using Grpc.Core;
using GStore;

namespace Server
{
    public class PuppetMasterService : PuppetMaster.PuppetMasterBase
    {

        private ServerService server;

        public PuppetMasterService(ServerService server)
        {
            this.server = server;
        }

        public override Task<StatusInfo> Status(StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.status(request));
        }

        public override Task<FreezeResponse> Freeze(FreezeRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.freeze(request));
        }

        public override Task<ReplicationResponse> Replication(ReplicationRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.replication(request));
        }
        public override Task<PartitionResponse> Partition(PartitionRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.partition(request));
        }
        public override Task<CrashResponse> Crash(CrashRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.crash(request));
        }
        public override Task<UnfreezeResponse> Unfreeze(UnfreezeRequest request, ServerCallContext context)
        {
            return Task.FromResult(server.unfreeze(request));
        }

    }
}