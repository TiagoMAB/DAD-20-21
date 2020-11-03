using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Status : Command {
        public Status(PuppetMaster form) : base(form) { }

        protected override async Task DoWork() {
            List<Task> tasks = new List<Task>();

            List<KeyValuePair<string, string>> ids = ConnectionInfo.GetAll();

            StatusRequest request = new StatusRequest { };

            foreach (var pair in ids) {
                tasks.Add(Task.Run(async () => {
                    GrpcChannel channel = GrpcChannel.ForAddress(pair.Value);

                    GStore.PuppetMaster.PuppetMasterClient client = new GStore.PuppetMaster.PuppetMasterClient(channel);

                    Random random = new Random();
                    String command = String.Format("Get status of '{0}'", pair.Key);

                    int remaining = TRIES;
                    do {
                        try {
                            StatusInfo response = await client.StatusAsync(request);

                            Log(StatusImpl.PrettyStatus(response));

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
                        Log(String.Format("MAX TRIES EXCEEDED: {0}\nAssuming '{1}' is dead", command, pair.Key));
                        this.form.RemoveConnection(pair.Key);
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            Log("All status received");
        }
    }
}
