using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Server
{
    public class PuppetMasterService : PuppetMaster.PuppetMasterBase
    {

        private readonly GStoreService service;

        public PuppetMasterService(GStoreService service)
        {
            this.service = service;
        }

        public override Task<StatusResponse> Status(StatusRequest request, ServerCallContext context)
        {
            Console.WriteLine("Status request received");

            //TO DO: implement
            return Task.FromResult(new StatusResponse());
        }

        public override Task<FreezeResponse> Freeze(FreezeRequest request, ServerCallContext context)
        {
            Console.WriteLine("Freeze request received");
            service.freeze();
            return Task.FromResult(new FreezeResponse());
        }

        public override Task<ReplicationResponse> Replication(ReplicationRequest request, ServerCallContext context)
        {
            Console.WriteLine("Replication request received");

            //TO DO: implement
            return Task.FromResult(new ReplicationResponse());
        }
        public override Task<PartitionResponse> Partition(PartitionRequest request, ServerCallContext context)
        {
            Console.WriteLine("Partition request received");

            //TO DO: implement
            return Task.FromResult(new PartitionResponse());
        }
        public override Task<CrashResponse> Crash(CrashRequest request, ServerCallContext context)
        {
            Console.WriteLine("Crash request received");

            //TO DO: implement
            return Task.FromResult(new CrashResponse());
        }
        public override Task<UnfreezeResponse> Unfreeze(UnfreezeRequest request, ServerCallContext context)
        {
            Console.WriteLine("Unfreeze request received");

            service.unfreeze();
            return Task.FromResult(new UnfreezeResponse());
        }

    }
}