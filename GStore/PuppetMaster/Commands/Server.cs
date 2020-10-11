using Grpc.Core;
using GStore;
using System;

namespace PuppetMaster.Commands {
    public class Server : Command {
        private readonly string id;
        private readonly string URL;
        private readonly int minDelay;
        private readonly int maxDelay;

        public Server(PuppetMaster form, string id, string URL, int minDelay, int maxDelay) : base(form) {
            this.id = id;
            this.URL = URL;
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
        }

        protected override void DoWork() {
            ConnectionInfo.AddServer(this.id, this.URL);

            Channel channel = new Channel(this.URL, ChannelCredentials.Insecure);

            PCS.PCSClient client = new PCS.PCSClient(channel);

            try {
                client.Server(new ServerRequest { Id = this.id, MaxDelay = this.maxDelay, MinDelay = this.minDelay });
            } catch (RpcException e) {
                // TODO: Improve error handling
                System.Diagnostics.Debug.WriteLine(e);
            }

            channel.ShutdownAsync().Wait();

            Log(String.Format("Server '{0}' listening at '{1}'", this.id, this.URL));
        }
    }
}
