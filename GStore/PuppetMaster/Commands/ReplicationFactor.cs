﻿using Grpc.Core;
using Grpc.Net.Client;
using GStore;
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

            foreach(var pair in ids) {
                tasks.Add(Task.Run(async () => { 
                    GrpcChannel channel = GrpcChannel.ForAddress(pair.Value);

                    GStore.PuppetMaster.PuppetMasterClient client = new GStore.PuppetMaster.PuppetMasterClient(channel);

                    try {
                        await client.ReplicationAsync(request);

                        Log(String.Format("ReplicationFactor set to {0} on '{1}'", this.factor, pair.Key));
                    } catch (RpcException e) {
                        String command = String.Format("Set replication factor on '{0}'", pair.Key);

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
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            Log(String.Format("ReplicationFactor set to {0} on all servers", this.factor));
        }
    }
}
