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

            Random random = new Random();
            String command = String.Format("Freeze server '{0}'", this.id);

            int remaining = TRIES;
            do {

                try {
                    await client.FreezeAsync(new FreezeRequest { });

                    Log(String.Format("Freezed server '{0}'", this.id));

                    await channel.ShutdownAsync();
                    return;
                } catch (RpcException e) {
                    switch (e.StatusCode) {
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

                remaining -= 1;
                if (remaining != 0) {
                    Log(String.Format("RETRYING: {0}...", command));
                }

                await Task.Delay(random.Next(MIN_BACKOFF, MAX_BACKOFF));
            } while (remaining != 0);

            if(remaining == 0) {
                Log(String.Format("MAX TRIES EXCEEDED: {0}\nAssuming server '{1}' is dead", command, this.id));
                this.form.RemoveServer(this.id);
            }
        }
    }
}
