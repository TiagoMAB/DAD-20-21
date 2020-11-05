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
        public override Task<StatusInfo> Status(
            StatusRequest request, ServerCallContext context)
        {
            ServerInfo server = ServerInfo.Instance();

            Console.WriteLine("Sending status to PuppetMaster...\n");

            return Task.FromResult(new StatusInfo { Id = server.UserName, Client = new ClientStatus { IsProcessComplete = server.ExecFinish } } );
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Script script;
            Server server;
            ServerInfo serverInfo;

            if (args.Length != 5)
            {
                Console.WriteLine("Incorrect arguments\n" +
                    "Correct arguments' format: username clientURL scriptPath serverId serverURL");
                Console.ReadKey(); return;
            }

            string[] details = args[1].Split("//");
            if (details.Length == 2)
                details = details[1].Split(':');
            else if (details.Length == 1)
                details = details[0].Split(':');
            else {
                Console.WriteLine("Unknown URL format \"{0}\"", args[1]);
                Console.ReadKey(); return;
            }

            //USERNAME
            Console.WriteLine("Client with username \"{0}\" started in host \"{1}\" and port \"{2}\".\n", args[0], details[0], details[1]);

            serverInfo = ServerInfo.Instance();

            serverInfo.AddServerURL(args[3], args[4]);
            serverInfo.UserName = args[0];
            serverInfo.CurrentServerURL = args[4];
            serverInfo.ExecFinish = false;

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            server = new Server
            {
                Services = { PuppetMaster.BindService(new ServerService()) },
                Ports = { new ServerPort(details[0], int.Parse(details[1]), ServerCredentials.Insecure) }
            };

            server.Start();

            try
            {
                script = Parser.ParseScript(args[2] + ".txt");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey(); return;
            }
            catch (InvalidExpressionException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey(); return;
            }
            catch (CycleInceptionException)
            {
                Console.WriteLine("It is not possible to have a cycle inside another cycle.");
                Console.ReadKey(); return;
            }
            catch (NoEndOfCycleException)
            {
                Console.WriteLine("Script ended with no cycle closure.");
                Console.ReadKey(); return;
            }

            try
            {
                serverInfo.GetServerInfo();
                script.Execute();
            }
            catch (NonExistentServerException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey(); return;
            } 
            catch (RpcException)
            {
                Console.ReadKey(); return;
            }

            serverInfo.ExecFinish = true;

            Console.WriteLine("Press any key to stop the client.");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
