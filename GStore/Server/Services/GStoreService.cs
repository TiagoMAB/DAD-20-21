using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using GStore;
using System.Diagnostics;
using System.Linq;

namespace Server
{
    public class GStoreService : GStore.GStore.GStoreBase
    {
        private readonly string id;
        private readonly string URL;
        private Dictionary<string, string> network = new Dictionary<string, string>();                                  //  Dictionary<server_id, URL>
        private Dictionary<string, List<string>> partitions = new Dictionary<string, List<string>>();                   //  Dictionary<partition_id, List<server_id>>
        private Dictionary<string, string> masters = new Dictionary<string, string>();                                  //  Dictionary<partition_id, master_id>
        private Dictionary<Tuple<String, String>, string> objects = new Dictionary<Tuple<String, String>, string>();    //  Dictionary<<partition_id, object_id>, value>
        private HashSet<string> own_partitions = new HashSet<string>();                                                 //  Hashset<partition_id>
        private bool frozen = false;
        private static Mutex m = new Mutex();

            
        public GStoreService(string id, string URL, string otherId, string otherURL)
        {
            this.id = id;
            this.URL = URL;

            //if server is the first in the network, handshake is not called
            if (otherId == null) 
            {
                return;
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);   //TO DO: why is this necessary?
            GrpcChannel channel = GrpcChannel.ForAddress(otherURL);
            GStore.GStore.GStoreClient client = new GStore.GStore.GStoreClient(channel);
            HandshakeReply reply = client.handshake(new HandshakeRequest { Id = id, Url = URL });

            foreach (HandshakeReply.Types.Info info in reply.Network) 
            {
                this.network.Add(info.Id, info.Url);    //TO DO: add lock if necessary, probably not

                channel = GrpcChannel.ForAddress(info.Url);
                client = new GStore.GStore.GStoreClient(channel);
                client.register(new RegisterRequest { Id = this.id, Url = this.URL }); //TODO: check return value, if necessary

            }

            this.network.Add(otherId, otherURL);    //TO DO: add lock if necessary, probably not

            //DEBUG:
            foreach (KeyValuePair<string, string> server in this.network)
            {
                Console.WriteLine("Server with id - " + server.Key + " - with url - " + server.Value);
            }
        }

        public override Task<GStore.HandshakeReply> handshake(HandshakeRequest request, ServerCallContext context)
        {
            Console.WriteLine("Handshake request received");
            if (frozen)
            {
                m.WaitOne();
                m.ReleaseMutex();
            }

            HandshakeReply reply = new HandshakeReply();
            foreach (KeyValuePair<string, string> server in this.network) 
            {
                reply.Network.Add(new HandshakeReply.Types.Info { Id = server.Key, Url = server.Value });
            }

            network.Add(request.Id, request.Url);
            Console.WriteLine("Handshake reply sent");
            return Task.FromResult(reply);
        }

        public override Task<GStore.RegisterReply> register(RegisterRequest request, ServerCallContext context)
        {
            Console.WriteLine("Register request received");
            if (frozen)
            {
                m.WaitOne();
                m.ReleaseMutex();
            }

            try
            {
                network.Add(request.Id, request.Url);

                //DEBUG:
                foreach (KeyValuePair<string, string> server in this.network)
                {
                    Console.WriteLine("Server with id - " + server.Key + " - with url - " + server.Value);
                }

                return Task.FromResult(new RegisterReply { Ok = true });
            }
            catch (Exception)
            {
                return Task.FromResult(new RegisterReply { Ok = false });
            }
        }

        public override Task<GStore.WriteReply> write(WriteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Write");
            return Task.FromResult(new WriteReply());
        }

        public override Task<GStore.ReadReply> read(ReadRequest request, ServerCallContext context)
        {
            Console.WriteLine("Read");
            return Task.FromResult(new ReadReply());
        }

        public override Task<GStore.ServerInfoReply> serverInfo(ServerInfoRequest request, ServerCallContext context)
        {
            Console.WriteLine("ServerInfo");
            ServerInfoReply reply = new ServerInfoReply();
            reply.Servers.Add(new ServerInfoReply.Types.Server { Id = this.id, Url = this.URL });

            foreach (KeyValuePair<string, string> server in this.network)
            {
                reply.Servers.Add(new ServerInfoReply.Types.Server { Id = server.Key , Url = server.Value });
            }

            foreach (KeyValuePair<string, List<string>> partition in this.partitions)
            {
                var partition1 = new ServerInfoReply.Types.Partition { Name = partition.Key, Master = partition.Value.First() };
                foreach (string server in partition.Value)
                {
                    partition1.Partitions.Add(server);
                }

                reply.Partition.Add(partition1);
            }

            return Task.FromResult(reply);
        }

        public void freeze()
        {
            m.WaitOne();
            frozen = true;
            Console.WriteLine("Frozen = true");
        }

        public void unfreeze()
        {
            m.ReleaseMutex();
            frozen = false;
            Console.WriteLine("Frozen = false");
        }

        public void crash()
        {
            Console.WriteLine("Process about to die");
            Environment.Exit(-1);
        }

        public void partition(PartitionRequest request)
        {
            string partition_name = request.Name;
            string master_id = request.Ids.First();
            List<string> server_ids = request.Ids.ToList();

            if (partitions.ContainsKey(partition_name))
            {
                return;
            }

            masters.Add(partition_name, master_id);
            partitions.Add(partition_name, server_ids);
            

            foreach (string id in this.network.Keys)
            {
                if (id == this.id)
                {
                    continue;
                }

                string server_url = network[id];
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                GrpcChannel channel = GrpcChannel.ForAddress(server_url);
                PuppetMaster.PuppetMasterClient client = new PuppetMaster.PuppetMasterClient(channel);
                var send = new PartitionRequest { Name = request.Name };
                send.Ids.AddRange(server_ids);
                client.Partition(send); 

            }
        }

        public void status()
        {
            Console.WriteLine("Known Servers:");
            foreach (KeyValuePair<string, string> server in this.network)
            {
                Console.WriteLine("Id: " + server.Key + " Url: " + server.Value);
            }
            Console.WriteLine("Known partitions:");
            foreach (KeyValuePair<string, List<string>> partition in this.partitions)
            {
                Console.Write("Partition: " + partition.Key + " Servers: ");
                foreach (string server in partition.Value)
                {
                    Console.Write(server + " ; ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Known masters:");
            foreach (KeyValuePair<string, string> server in this.masters)
            {
                Console.WriteLine("Partition: " + server.Key + " Master: " + server.Value);
            }

        }
    }
}