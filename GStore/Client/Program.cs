using System;
using System.IO;
using Client.Commands;
using Client.Exceptions;
using GStore;
using Grpc.Core;
using System.Threading.Tasks;

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

            /*TODO: Probably args still needed when only clients and servers exist ||| for example, number of replicas, etc etc*/

            /*
             clientHost: host name required for status commands (client is actually the server in this operation)
             clientPort: port number required for status command (client is actually the server in this operation)
             serverHost: first host name being contacted by the server
             serverPort: first port number being contacted by the server
             */
            if (args.Length != 4)
            {
                Console.WriteLine("Incorrect arguments\n" +
                    "Correct arguments' format: clientHost clientPort serverHost serverPort");
                return;
            }

            serverInfo = ServerInfo.Instance();

            serverInfo.CurrentHost = args[2];
            serverInfo.CurrentPort = int.Parse(args[2]);
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

            script.Execute();

            serverInfo.ExecFinish = true;

            Console.WriteLine("Press any key to stop the client.");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
