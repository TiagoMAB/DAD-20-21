using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using PuppetMaster.Exceptions;
using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Freeze : Command {
        private readonly string id;

        public Freeze(PuppetMaster form, string id) : base(form) {
            this.id = id;
        }
        protected override async Task DoWork() {
            String URL = ConnectionInfo.GetServer(this.id);

            if(URL == null) {
                Log(String.Format("ERROR: Unknown server '{0}' on command Freeze", this.id));
                throw new UnknownServerException("Freeze", this.id);
            }

            GrpcChannel channel = GrpcChannel.ForAddress(URL);

            GStore.PuppetMaster.PuppetMasterClient client = new GStore.PuppetMaster.PuppetMasterClient(channel);

            try {
                await client.FreezeAsync(new FreezeRequest { } );

                Log(String.Format("Freezed server '{0}'", this.id));

                await channel.ShutdownAsync();
            } catch (RpcException e) {
                String command = String.Format("Freeze server '{0}'", this.id);

                switch(e.StatusCode) {
                    case StatusCode.Aborted:
                        Log(String.Format("ABORTED: {0}", command));
                        break;
                    case StatusCode.Cancelled:
                        Log(String.Format("CANCELLED: {0}", command));
                        break;
                    case StatusCode.DeadlineExceeded:
                        Log(String.Format("TIMEOUT: {0}", command));
                        break;
                    case StatusCode.Internal:
                        Log(String.Format("INTERNAL ERROR: {0}", command));
                        break;
                    default:
                        Log(String.Format("UNKNOWN ERROR: {0}", command));
                        break;
                }
            }
        }
    }
}
