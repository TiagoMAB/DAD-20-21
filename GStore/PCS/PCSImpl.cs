using Grpc.Core;
using GStore;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PCS {
    public class PCSImpl : GStore.PCS.PCSBase {
        private readonly string client;
        private readonly string server;

        public PCSImpl(string client, string server) {
            this.client = client;
            this.server = server;
        }

        public override Task<ClientResponse> Client(ClientRequest request, ServerCallContext context) {
            Console.WriteLine(String.Format("[{1}] Request to launch client with script file '{0}'", request.Script, DateTime.Now.ToString()));

            try {
                ProcessStartInfo p = new ProcessStartInfo("cmd.exe", String.Format("/c start {0} {1} {2} {3} {4} {5}", this.client, request.Id, request.ClientUrl, request.Script, request.ServerId, request.ServerUrl));
                p.WorkingDirectory = Path.GetDirectoryName(this.client);
                if (Process.Start(p) == null) {
                    // TODO: handle error
                }
            } catch(Exception e) {
                Console.WriteLine(e);
            }

            return Task.FromResult(new ClientResponse());
        }

        public override Task<ServerResponse> Server(ServerRequest request, ServerCallContext context) {
            Console.WriteLine(String.Format("[{4}] Request to launch server with id '{0}' at '{1}' with a min delay of {2}ms and max delay of {3}ms", request.Id, request.Url, request.MinDelay, request.MaxDelay, DateTime.Now.ToString()));

            try {
                ProcessStartInfo p = new ProcessStartInfo("cmd.exe", String.Format("/c start {0} {1} {2} {3} {4} {5} {6}", this.server, request.Id, request.Url, request.MinDelay, request.MaxDelay, request.OtherId, request.OtherUrl));
                if (Process.Start(p) == null) {
                    // TODO: handle error
                }
            } catch(Exception e) {
                Console.WriteLine(e);
            }

            return Task.FromResult(new ServerResponse());
        }
    }
}
