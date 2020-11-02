using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using PuppetMaster.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Server : Command {
        private readonly string id;
        private readonly string host;
        private readonly string port;
        private readonly int minDelay;
        private readonly int maxDelay;

        public Server(PuppetMaster form, string id, string URL, int minDelay, int maxDelay) : base(form) {
            string[] address = URL.Split(':');

            if(!URL.StartsWith("http://") || address.Length != 3) {
                throw new InvalidURLException("Server", URL);
            }

            this.id = id;
            this.host = address[0] + ":" + address[1];
            this.port = address[2];
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
        }

        protected override async Task DoWork() {
            string URL = this.host + ":" + this.port;

            GrpcChannel channel = GrpcChannel.ForAddress(this.host + ":10000");

            PCS.PCSClient client = new PCS.PCSClient(channel);

            KeyValuePair<string, string> connectURL = ConnectionInfo.GetRandomServer();

            try {
                this.form.AddServer(this.id, URL);

                await client.ServerAsync(new ServerRequest { Id = this.id, Url = URL, MaxDelay = this.maxDelay, MinDelay = this.minDelay, OtherId = connectURL.Key, OtherUrl = connectURL.Value  });

                Log(String.Format("Server '{0}' listening at '{1}'", this.id, URL));

                // FIXME: await or no shutdown?
                channel.ShutdownAsync().Wait();
            } catch (RpcException e) {
                String command = String.Format("Create server '{0}' at '{1}'", this.id, URL);

                ConnectionInfo.RemoveServer(this.id);

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
