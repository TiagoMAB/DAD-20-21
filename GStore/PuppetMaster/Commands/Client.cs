using Grpc.Core;
using GStore;
using System;

namespace PuppetMaster.Commands {
    public class Client : Command {
        private readonly string username;
        private readonly string URL;
        private readonly string file;

        public Client(string username, string URL, string file) {
            this.username = username;
            this.URL = URL;
            this.file = file;
        }

        public void Execute() {
            Channel channel = new Channel(this.URL, ChannelCredentials.Insecure);

            PCS.PCSClient client = new PCS.PCSClient(channel);

            try {
                client.ClientAsync(new ClientRequest { Script = this.file });
            } catch (RpcException e) {
                // TODO: Improve error handling
                System.Diagnostics.Debug.WriteLine(e);
            }

            channel.ShutdownAsync();
            System.Diagnostics.Debug.WriteLine(String.Format("[{0}] DONE", DateTime.Now.ToString()));
        }
    }
}
