using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using PuppetMaster.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Partition : Command {
        private readonly string name;
        private readonly List<string> replicas;

        public Partition(PuppetMaster form, int num, string name, IEnumerable<string> replicas) : base(form) {
            this.name = name;
            this.replicas = new List<string>(replicas);

            if (num != this.replicas.Count) {
                throw new PartitionParameterNumberMismatchException(num, this.replicas.Count);
            }
        }

        protected override async Task DoWork() {
            string print = String.Format("Partition {0} created\nShared by {1} replicas: ", this.name, this.replicas.Count);

            foreach (string id in this.replicas) {
                String URL = ConnectionInfo.GetServer(id);

                if (URL == null) {
                    throw new UnknownServerException("Partition", id);
                }

                print = String.Concat(print, String.Format("{0} ", id));
            }

            PartitionRequest request = new PartitionRequest { Replicas = this.replicas.Count, Name = this.name, Ids = { this.replicas } };

            String master = ConnectionInfo.GetServer(this.replicas[0]);

            GrpcChannel channel = GrpcChannel.ForAddress(master);

            GStore.PuppetMaster.PuppetMasterClient client = new GStore.PuppetMaster.PuppetMasterClient(channel);

            try {
                await client.PartitionAsync(request);

                Log(String.Format("Created partition '{0}'", this.name));
            } catch (RpcException e) {
                String command = String.Format("Create partition '{0}'", this.name);

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
        }
    }
}
