using System;
using GStore;
using Grpc.Core;
using System.Threading;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Linq;
using Domain;
using System.Threading.Tasks;

namespace Server 
{

    public class ServerService {

        private readonly string id;
        private readonly string URL;

        private Dictionary<string, string> network = new Dictionary<string, string>();                                  //  Dictionary<server_id, URL>
        private Dictionary<string, Partition> partitions = new Dictionary<string, Partition>();                         //  Dictionary<partition_id, Partition>

        private bool frozen = false;
        private readonly object key = new object();

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

            Task t = Task.Run(() => 
            { 
                lock (key) 
                {
                    frozen = true;
                    Console.WriteLine("Freezing");
                    Monitor.Wait(key);
                    Console.WriteLine("Finished Freezing");
                    Monitor.Pulse(key);
                    Console.WriteLine("Pulsing");
                } 
            });
            
            Console.WriteLine("Frozen = true");
            return new FreezeResponse();
        }

        public UnfreezeResponse unfreeze(UnfreezeRequest request)
        {
            Console.WriteLine("Unfreeze request received");

            lock (key)
            {
                Monitor.Pulse(key);
            }

            frozen = false;
            Console.WriteLine("Frozen = false");
            return new UnfreezeResponse();
        }
        public ReplicationResponse replication(ReplicationRequest request)
        {
            Console.WriteLine("Replication request received");

            //TO DO: implement
            return new ReplicationResponse();
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
            Partition partition = partitions[partitionId];

            if (partition.masterID != this.id)
            {
                //TO DO: only masters can write an object
            }

            string objectId = request.ObjectId;
            string value = request.Value;

            lock (partition)
            {
                while (partition.locked == true)
                {
                    Monitor.Wait(partition);        //Write request is queued until lock is obtained
                }
                partition.locked = true;
            }

            foreach (string url in partition.replicas.Values)
            {
                if (url == this.URL) 
                { 
                    continue; 
                }

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                GrpcChannel channel = GrpcChannel.ForAddress(url);
                var client = new ServerCommunication.ServerCommunicationClient(channel);
                LockObjectReply reply = client.LockObject(new LockObjectRequest { PartitionId = partitionId, ObjectId = objectId });                   //TO DO: do it async and evaluate return value

            }

            partition.addObject(objectId, value);

            foreach (string url in partition.replicas.Values)
            {
                if (url == this.URL)
                {
                    continue;
                }

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);   //TO DO: why is this necessary?
                GrpcChannel channel = GrpcChannel.ForAddress(url);
                var client = new ServerCommunication.ServerCommunicationClient(channel);
                WriteObjectReply reply = client.WriteObject(new WriteObjectRequest { PartitionId = partitionId, ObjectId = objectId, Value = value });                   //TO DO: do it async and evaluate return value
            }

            lock (partition)
            {
                Monitor.Pulse(partition);
                partition.locked = false;
            }

            return new WriteReply();
        }

        public ReadReply read(ReadRequest request)
        {
            Console.WriteLine("Read");

            string partitionId = request.PartitionId;
            string objectId = request.ObjectId;

            if (!partitions.ContainsKey(partitionId))
            {
                return new ReadReply { Value = "N/A" };                 // TO DO: handle failure (partition doesn't exist)
            }
            else
            {
                Partition p = partitions[partitionId];
                string value;

                lock (p)
                {
                    while (p.locked == true)
                    {
                        Monitor.Wait(p);        //Write request is queued until lock is obtained
                    }
                    value = p.getObject(objectId);
                    Monitor.Pulse(p);
                }

                return new ReadReply { Value = value };
            }
        }

        public ListServerReply listServer(ListServerRequest request)
        {
            Console.WriteLine("listServer");
            ListServerReply reply = new ListServerReply();

            foreach (Partition p in partitions.Values)
            {
                bool isMaster = p.masterID == this.id;
                foreach (KeyValuePair<string, string> o in p.objects)
                {
                    reply.Values.Add(new ListServerReply.Types.ListValue { PartitionId = p.name, ObjectId = o.Key, Value = o.Value, IsMaster = isMaster });
                } 
            }

            return reply;
        }

        //
        //Server-Server Communication
        //

        public LockObjectReply lockObject(LockObjectRequest request)        //TO DO: handle failures
        {
            bool ok;
            string partitionId = request.PartitionId;

            lock (partitions[partitionId])
            {
                if (partitions[partitionId].locked == true)
                {
                    ok = false;                             //partition is locked (theoretically can't happen, TO DO: maybe remove)
                    Console.WriteLine("Partition " + partitionId + " is already locked by another request");
                }
                else
                {
                    partitions[partitionId].locked = true;  //partition will be locked
                    ok = true;
                    Console.WriteLine("Partition " + partitionId + " was locked successfully");
                }
            }

            return new LockObjectReply { Ok = ok };
        }

        public WriteObjectReply writeObject(WriteObjectRequest request)     //TO DO: handle failures
        {
            string partitionId = request.PartitionId;
            string objectId = request.ObjectId;
            string value = request.Value;
            
            lock (partitions[partitionId])
            {
                partitions[partitionId].addObject(objectId, value);
                partitions[partitionId].locked = false;
            }

            return new WriteObjectReply { Ok = true };
        }

        public HandshakeReply handshake(HandshakeRequest request)
        {
            Console.WriteLine("Handshake request received");

            if (frozen)
            {
                lock(key)
                {
                    Console.WriteLine("Handshake Freezed");
                    Monitor.Wait(key);
                    Console.WriteLine("Handshake Unfreezed");
                    Monitor.Pulse(key);
                    Console.WriteLine("Handshake Pulsing");
                }
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
                lock (key)
                {
                    Console.WriteLine("Register Freezed");
                    Monitor.Wait(key);
                    Console.WriteLine("Register Unfreezed");
                    Monitor.Pulse(key);
                    Console.WriteLine("Register Pulsing");
                }
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