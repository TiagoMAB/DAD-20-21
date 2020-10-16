using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands
{
    public class Status : Command
    {
        public Status(PuppetMaster form) : base(form) { }

        protected override async Task DoWork()
        {
            List<Task> tasks = new List<Task>();

            List<KeyValuePair<string, string>> ids = ConnectionInfo.GetAll();

            StatusRequest request = new StatusRequest { };

            foreach(var pair in ids) {
                tasks.Add(Task.Run(async () => { 
                    GrpcChannel channel = GrpcChannel.ForAddress(pair.Value);

                    GStore.PuppetMaster.PuppetMasterClient client = new GStore.PuppetMaster.PuppetMasterClient(channel);

                    try {
                        await client.StatusAsync(request);

                        // TODO: Print status of each client
                        Log(String.Format("Got status of '{0}'", pair.Key));
                    } catch (RpcException e) {
                        String command = String.Format("Get status of '{0}'", pair.Key);

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

            await Task.WhenAll(tasks);

            Log("All status received");
        }
    }
}
