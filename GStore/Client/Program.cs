﻿using System;
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

            /*
             clientHost: host name required for status commands (client is actually the server in this operation)
             clientPort: port number required for status command (client is actually the server in this operation)
             serverHost: first server URL being contacted by the client
             */
            if (args.Length != 6)
            {
                Console.WriteLine("Incorrect arguments\n" +
                    "Correct arguments' format: username clientHost clientPort scriptPath serverId serverURL");
                return;
            }

            //USERNAME
            Console.WriteLine("Client with username \"{0}\" started.", args[0]);

            serverInfo = ServerInfo.Instance();

            serverInfo.AddServerURL(args[4], args[5]);
            serverInfo.UserName = args[0];
            serverInfo.CurrentServerURL = args[5];
            serverInfo.ExecFinish = false;

            server = new Server
            {
                Services = { PuppetMaster.BindService(new ServerService()) },
                Ports = { new ServerPort(args[1], int.Parse(args[2]), ServerCredentials.Insecure) }
            };

            server.Start();

            try
            {
                script = Parser.ParseScript(args[3]);
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

            serverInfo.GetServerInfo();

            script.Execute();

            serverInfo.ExecFinish = true;

            Console.WriteLine("Press any key to stop the client.");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
