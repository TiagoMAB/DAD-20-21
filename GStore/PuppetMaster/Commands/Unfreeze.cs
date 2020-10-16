using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using PuppetMaster.Exceptions;
using System;

namespace PuppetMaster.Commands {
    public class Unfreeze : Command {
        private readonly string id;

        public Unfreeze(PuppetMaster form, string id) : base(form) {
            this.id = id;
        }

        protected override void DoWork() {
            String URL = ConnectionInfo.GetServer(this.id);

            if(URL == null) {
                throw new UnknownServerException("Unfreeze", this.id);
            }

            GrpcChannel channel = GrpcChannel.ForAddress(URL);

            GStore.PuppetMaster.PuppetMasterClient client = new GStore.PuppetMaster.PuppetMasterClient(channel);

            try {
                client.Freeze(new FreezeRequest { } );
            } catch (RpcException e) {
                String command = String.Format("Unfreeze server '{0}'", this.id);

                switch(e.StatusCode) {
                    case StatusCode.Aborted:
                        Log(String.Format("ABORTED: {0}", command));
                        return;
                    case StatusCode.Cancelled:
                        Log(String.Format("CANCELLED: {0}", command));
                        return;
                    case StatusCode.DeadlineExceeded:
                        Log(String.Format("TIMEOUT: {0}", command));
                        return;
                    case StatusCode.Internal:
                        Log(String.Format("INTERNAL ERROR: {0}", command));
                        return;
                    default:
                        Log(String.Format("UNKNOWN ERROR: {0}", command));
                        return;
                }
            }
            Log(String.Format("Unfreezed server '{0}'", this.id));
        }
    }
}
