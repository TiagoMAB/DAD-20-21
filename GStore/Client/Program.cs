using System;
using System.IO;
using Client.Commands;
using Client.Exceptions;
using GStore;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;
using System.Linq;

namespace Client
{

    public class ServerService : PuppetMaster.PuppetMasterBase
    {
        public override Task<StatusResponse> Status(
            StatusRequest request, ServerCallContext context)
        {
            ServerInfo server = ServerInfo.Instance();

            return Task.FromResult(new StatusResponse { IsClient = true, IsProcessComplete = server.ExecFinish } );
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Script script;
            Server server;
            ServerInfo serverInfo;

            /*
             clientHost: host name required for status commands (client is actually the server in this operation)
             clientPort: port number required for status command (client is actually the server in this operation)
             serverHost: first server URL being contacted by the client
             */
            if (args.Length != 3)
            {
                Console.WriteLine("Incorrect arguments\n" +
                    "Correct arguments' format: clientHost clientPort serverHost serverPort");
                return;
            }

            serverInfo = ServerInfo.Instance();

            serverInfo.CurrentServerURL = args[2];
            serverInfo.ExecFinish = false;

            server = new Server
            {
                Services = { PuppetMaster.BindService(new ServerService()) },
                Ports = { new ServerPort(args[0], int.Parse(args[1]), ServerCredentials.Insecure) }
            };

            server.Start();

            try
            {
                script = Parser.ParseScript();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (InvalidExpressionException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (CycleInceptionException)
            {
                Console.WriteLine("It is not possible to have a cycle inside another cycle.");
                return;
            }
            catch (NoEndOfCycleException)
            {
                Console.WriteLine("Script ended with no cycle closure.");
                return;
            }

            GetServerInfo();

            script.Execute();

            serverInfo.ExecFinish = true;

            Console.WriteLine("Press any key to stop the client.");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }

        private static void GetServerInfo()
        {
            ServerInfo server = ServerInfo.Instance();

            //Ask other servers' details
            var channel = GrpcChannel.ForAddress(server.CurrentServerURL);
            var client = new GStore.GStore.GStoreClient(channel);

            var response = client.serverInfo( new ServerInfoRequest {} );

            foreach (var value in response.Servers) {
                server.AddServer(value.ServerId, value.Url, value.MasterPartitionId.ToList(), value.Partitions.ToList());
            }
        }
    }
}
