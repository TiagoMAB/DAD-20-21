using Grpc.Core;
using GStore;
using System;
using System.Threading.Tasks;

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

        protected override void DoWork() {
            ConnectionInfo.AddClient(this.username, this.URL);

            Channel channel = new Channel(this.URL, ChannelCredentials.Insecure);

            PCS.PCSClient client = new PCS.PCSClient(channel);

            try {
                client.Client(new ClientRequest { Script = this.file });
            } catch (RpcException e) {
                // TODO: Improve error handling
                System.Diagnostics.Debug.WriteLine(e);
            }

            channel.ShutdownAsync().Wait();
            System.Diagnostics.Debug.WriteLine(String.Format("[{0}] DONE", DateTime.Now.ToString()));
        }
    }
}
