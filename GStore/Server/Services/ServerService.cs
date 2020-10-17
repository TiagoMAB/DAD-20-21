using System;
using GStore;
using Grpc.Core;
using System.Threading;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Linq;
using Domain;

namespace Server 
{

    public class ServerService {

        private readonly string id;
        private readonly string URL;

        private Dictionary<string, string> network = new Dictionary<string, string>();                                  //  Dictionary<server_id, URL>
        private Dictionary<string, Partition> partitions = new Dictionary<string, Partition>();                         //  Dictionary<partition_id, Partition>

        private bool frozen = false;
        private static Mutex m = new Mutex();
    
        public ServerService(string id, string URL, string otherId, string otherURL)
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
            var client = new ServerCommunication.ServerCommunicationClient(channel);
            HandshakeReply reply = client.Handshake(new HandshakeRequest { Id = id, Url = URL });

            foreach (HandshakeReply.Types.Info info in reply.Network) 
            {
                this.network.Add(info.Id, info.Url);    //TO DO: add lock if necessary, probably not

                channel = GrpcChannel.ForAddress(info.Url);
                client = new ServerCommunication.ServerCommunicationClient(channel);
                client.Register(new RegisterRequest { Id = this.id, Url = this.URL }); //TODO: check return value, if necessary

            }

            this.network.Add(otherId, otherURL);    //TO DO: add lock if necessary, probably not
        }

        //
        //PuppetMaster-Server Communication
        //

        public StatusInfo status(StatusRequest request)
        {
            Console.WriteLine("Status request received");
            Console.WriteLine("Known Servers:");
            foreach (KeyValuePair<string, string> server in this.network)
            {
                Console.WriteLine("Id: " + server.Key + " Url: " + server.Value);
            }
            Console.WriteLine("Known partitions:");
            foreach (Partition p in this.partitions.Values)
            {
                Console.WriteLine(p.ToString());
               
            }

            return new StatusInfo();
        }

        public FreezeResponse freeze(FreezeRequest request)
        {
            Console.WriteLine("Freeze request received");
            m.WaitOne();
            frozen = true;
            Console.WriteLine("Frozen = true");
            return new FreezeResponse();
        }

        public ReplicationResponse replication(ReplicationRequest request)
        {
            Console.WriteLine("Replication request received");

            //TO DO: implement
            return new ReplicationResponse();
        }

        public UnfreezeResponse unfreeze(UnfreezeRequest request)
        {
            Console.WriteLine("Unfreeze request received");

            m.ReleaseMutex();
            frozen = false;
            Console.WriteLine("Frozen = false");
            return new UnfreezeResponse();
        }

        public CrashResponse crash(CrashRequest request)
        {
            Console.WriteLine("Crash request received");
            Console.WriteLine("Process about to die");
            Environment.Exit(-1);
            return new CrashResponse();
        }

        public PartitionResponse partition(PartitionRequest request)
        {
            Console.WriteLine("Partition request received");

            string partition_name = request.Name;
            string master_id = request.Ids.First();
            string master_url = master_id == this.id ? this.URL : network[master_id];
            List<string> server_ids = request.Ids.ToList();
            bool own = server_ids.Contains(this.id);

            Partition p = new Partition(partition_name, master_id, master_url, own);

            foreach (string id in server_ids)
            {
                p.addId(id, this.id == id ? this.URL : network[id]);
            }

            this.partitions.Add(partition_name, p);

            //if current server is the master, partition is sent to all other members of the network
            if (this.id == master_id)
            {
                foreach (string id in this.network.Keys)
                {
                    string server_url = network[id];
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    GrpcChannel channel = GrpcChannel.ForAddress(server_url);
                    PuppetMaster.PuppetMasterClient client = new PuppetMaster.PuppetMasterClient(channel);
                    var send = new PartitionRequest { Name = request.Name };
                    send.Ids.AddRange(server_ids);
                    client.Partition(send);
                }
            }

            return new PartitionResponse();
        }


        //
        //Client-Server Communication
        //

        public ServerInfoReply serverInfo(ServerInfoRequest request)
        {
            Console.WriteLine("ServerInfo");
            ServerInfoReply reply = new ServerInfoReply();
            reply.Servers.Add(new ServerInfoReply.Types.Server { Id = this.id, Url = this.URL });

            foreach (KeyValuePair<string, string> server in this.network)
            {
                reply.Servers.Add(new ServerInfoReply.Types.Server { Id = server.Key, Url = server.Value });
            }

            foreach (Partition p in this.partitions.Values)
            {
                var partition1 = new ServerInfoReply.Types.Partition { Name = p.name, Master = p.masterID };
                foreach (string server in p.replicas.Keys)
                {
                    partition1.ServerIds.Add(server);
                }

                reply.Partition.Add(partition1);
            }

            return reply;
        }

        public WriteReply write(WriteRequest request)
        {
            Console.WriteLine("Write");
            string partitionId = request.PartitionId;
            string objectId = request.ObjectId;
            string value = request.Value;
/*
            if (!ownPartitions.Contains(partitionId))
            {
                // TO DO: server doesn't belong to this partition
            }

            foreach (string id in partitions[partitionId]) 
            {

            }
*/
            return new WriteReply();
        }

        public ReadReply read(ReadRequest request)
        {
            Console.WriteLine("Read");
            return new ReadReply();
        }

        public ListServerReply listServer(ListServerRequest request)
        {
            Console.WriteLine("listServer");
            return new ListServerReply();
        }

        //
        //Server-Server Communication
        //

        public LockObjectReply lockObject(LockObjectRequest request)
        {
            bool taken = false;

            //Monitor.Enter(, taken);
            return new LockObjectReply { Ok = taken };
        }

        public WriteObjectReply writeObject(WriteObjectRequest request)
        {
            bool taken = false;

            return new WriteObjectReply { Ok = taken };
        }

        public HandshakeReply handshake(HandshakeRequest request)
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
            return reply;
        }

        public RegisterReply register(RegisterRequest request)
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
                return new RegisterReply { Ok = true };
            }
            catch (Exception)
            {
                return new RegisterReply { Ok = false };
            }
        }

    }
}