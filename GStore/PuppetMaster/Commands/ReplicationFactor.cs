using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class ReplicationFactor : Command {
        private readonly int factor;

        public ReplicationFactor(PuppetMaster form, int factor) : base(form) {
            this.factor = factor;
        }

        protected override async Task DoWork() {
            List<Task> tasks = new List<Task>();

            List<KeyValuePair<string, string>> ids = ConnectionInfo.GetAll();

            ReplicationRequest request = new ReplicationRequest { Factor = this.factor };

            foreach (var pair in ids) {
                tasks.Add(Task.Run(async () => {
                    GrpcChannel channel = GrpcChannel.ForAddress(pair.Value);

                    GStore.PuppetMaster.PuppetMasterClient client = new GStore.PuppetMaster.PuppetMasterClient(channel);

                    Random random = new Random();
                    String command = String.Format("Set replication factor on '{0}'", pair.Key);

                    int remaining = TRIES;
                    do {
                        try {
                            await client.ReplicationAsync(request);

                            Log(String.Format("ReplicationFactor set to {0} on '{1}'", this.factor, pair.Key));

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

                    if (remaining == 0) {
                        Log(String.Format("MAX TRIES EXCEEDED: {0}\nAssuming server '{1}' is dead", command, pair.Key));
                        this.form.RemoveServer(pair.Key);
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            Log(String.Format("ReplicationFactor set to {0} on all servers", this.factor));
        }
    }
}
