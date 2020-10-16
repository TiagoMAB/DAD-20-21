using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using PuppetMaster.Exceptions;
using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Client : Command {
        private readonly string username;
        private readonly string host;
        private readonly string port;
        private readonly string file;

        public Client(PuppetMaster form, string username, string URL, string file) : base(form) {
            string[] address = URL.Split(':');

            if(!URL.StartsWith("http://") || address.Length != 3) {
                throw new InvalidURLException("Client", URL);
            }

            this.username = username;
            this.host = address[0] + ":" + address[1];
            this.port = address[2];
            this.file = file;
        }

        protected override async Task DoWork() {
            string URL = this.host + ":" + this.port;

            ConnectionInfo.AddClient(this.username, URL);

            GrpcChannel channel = GrpcChannel.ForAddress(this.host + ":10000");

            PCS.PCSClient client = new PCS.PCSClient(channel);

            try {
                await client.ClientAsync(new ClientRequest { ClientUrl = URL, Script = this.file, ServerUrl = ConnectionInfo.GetRandomServer() });

                channel.ShutdownAsync().Wait();

                Log(String.Format("Client '{0}' started", this.username));
            } catch (RpcException e) {
                String command = String.Format("Create client '{0}' at '{1}'", this.username, URL);

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
