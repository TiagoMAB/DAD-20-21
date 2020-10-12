using Grpc.Core;
using GStore;
using System;

namespace PuppetMaster.Commands {
    public class Server : Command {
        private readonly string id;
        private readonly string host;
        private readonly string port;
        private readonly int minDelay;
        private readonly int maxDelay;

        public Server(PuppetMaster form, string id, string URL, int minDelay, int maxDelay) : base(form) {
            string[] address = URL.Split(':');

             // TODO: Assert correct format of address

            this.id = id;
            this.host = address[0];
            this.port = address[1];
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
        }

        protected override void DoWork() {
            string URL = this.host + ":" + this.port;

            ConnectionInfo.AddServer(this.id, URL);

            Channel channel = new Channel(this.host + ":10000", ChannelCredentials.Insecure);

            PCS.PCSClient client = new PCS.PCSClient(channel);

            try {
                client.Server(new ServerRequest { Id = this.id, Url = URL, MaxDelay = this.maxDelay, MinDelay = this.minDelay });
            } catch (RpcException e) {
                // TODO: Improve error handling
                System.Diagnostics.Debug.WriteLine(e);
            }

            channel.ShutdownAsync().Wait();

            Log(String.Format("Server '{0}' listening at '{1}'", this.id, URL));
        }
    }
}
