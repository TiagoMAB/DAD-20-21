using Grpc.Core;
using GStore;
using System;

namespace PuppetMaster.Commands {
    public class Client : Command {
        private readonly string username;
        private readonly string host;
        private readonly string port;
        private readonly string file;

        public Client(PuppetMaster form, string username, string URL, string file) : base(form) {
            string[] address = URL.Split(':');

            // TODO: Assert correct format of address

            this.username = username;
            this.host = address[0];
            this.port = address[1];
            this.file = file;
        }

        protected override void DoWork() {
            string URL = this.host + ":" + this.port;

            ConnectionInfo.AddClient(this.username, URL);

            Channel channel = new Channel(this.host + ":10000", ChannelCredentials.Insecure);

            PCS.PCSClient client = new PCS.PCSClient(channel);

            try {
                client.Client(new ClientRequest { ClientUrl = URL, Script = this.file, ServerUrl = ConnectionInfo.GetRandomServer() });
            } catch (RpcException e) {
                // TODO: Improve error handling
                System.Diagnostics.Debug.WriteLine(e);
            }

            channel.ShutdownAsync().Wait();

            Log(String.Format("Client '{0}' started", this.username));
        }
    }
}
