using System;
using GStore;
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
        private readonly int delay;

        private Dictionary<string, string> network = new Dictionary<string, string>();                                  //  Dictionary<server_id, URL>
        private Dictionary<string, Partition> partitions = new Dictionary<string, Partition>();                         //  Dictionary<partition_id, Partition>

        private bool frozen = false;
        private readonly object key = new object();

        public ServerService(string id, string URL, int delay)
        {
            this.id = id;
            this.URL = URL;
            this.delay = delay;
        }

        public ServerService(string id, string URL, int delay, string otherId, string otherURL)
        {
            this.id = id;
            this.URL = URL;
            this.delay = delay;

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(otherURL);
            ServerCommunication.ServerCommunicationClient client = new ServerCommunication.ServerCommunicationClient(channel);
            HandshakeReply reply = client.Handshake(new HandshakeRequest { Id = id, Url = URL });

            foreach (HandshakeReply.Types.Info info in reply.Network)
            {
                this.network.Add(info.Id, info.Url);

                channel = GrpcChannel.ForAddress(info.Url);
                client = new ServerCommunication.ServerCommunicationClient(channel);
                client.Register(new RegisterRequest { Id = this.id, Url = this.URL });

            }

            this.network.Add(otherId, otherURL);
        } 

        /*
         * 
         * PuppetMaster-Server Communication
         * 
         */

        /*
         * Prints relevant information about the current state of the server
         * This includes objects replicated by the server and all known servers and partitions in the network
         */
        public StatusInfo status(StatusRequest request)
        {
            delays();

            Console.WriteLine("Status()...");
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

        /*
         * Causes all messages received by the server to be put on a waiting queue, effectively freezing the server.
         */
        public FreezeResponse freeze(FreezeRequest request)
        {
            delays();

            Console.WriteLine("Freeze()...");
            Task t = Task.Run(() => 
            { 
                lock (key) 
                {
                    frozen = true;
                    Monitor.Wait(key);
                    Monitor.Pulse(key);
                } 
            });
            
            return new FreezeResponse();
        }

        /*
         * Causes all messages received by the server to stop being put on a waiting queue, effectively unfreezing the server.
         */
        public UnfreezeResponse unfreeze(UnfreezeRequest request)
        {
            delays();

            Console.WriteLine("Unfreeze()...");
            lock (key)
            {
                Monitor.Pulse(key);
                frozen = false;
            }

            return new UnfreezeResponse();
        }

        /*
         * Simulates a "crash" by exiting the process
         */
        public CrashResponse crash(CrashRequest request)
        {
            delays();

            //TO DO: Inform others of the crash
            //TO DO: Return before exit ???
            Console.WriteLine("Crash()...");
            Environment.Exit(-1);
            return new CrashResponse();
        }

        /*
         * Implement ???
         */
        public ReplicationResponse replication(ReplicationRequest request)
        {
            Console.WriteLine("Replication()...");

            return new ReplicationResponse();
        }

        /*
         * Sets up a new partition and sends the partition's information to the other servers
         */
        public PartitionResponse partition(PartitionRequest request)
        {
            delays();

            Console.WriteLine("Partition()...");
            string partition_name = request.Name;
            string master_id = request.Ids.First();

            if (this.id != master_id)
            {
                //TO DO: Only masters can receive partition requests, they will then share the information with the rest of the network
            }

            string master_url = this.URL;
            List<string> server_ids = request.Ids.ToList();
            bool own = server_ids.Contains(this.id);
            Partition p = new Partition(partition_name, master_id, master_url, own);

            foreach (string id in server_ids)
            {
                p.addId(id, this.id == id ? this.URL : network[id]);
            }

            this.partitions.Add(partition_name, p);

            //partition is sent to all other members of the network
            foreach (string id in this.network.Keys)
            {
                string server_url = network[id];

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                GrpcChannel channel = GrpcChannel.ForAddress(server_url);
                ServerCommunication.ServerCommunicationClient client = new ServerCommunication.ServerCommunicationClient(channel);
                SharePartitionRequest p_request = new SharePartitionRequest { Name = request.Name };
                p_request.Ids.AddRange(server_ids);
                client.SharePartition(p_request);
            }

            return new PartitionResponse();
        }


        //
        //Client-Server Communication
        //

        /*
         * Sends client the known network of servers
         */
        public ServerInfoReply serverInfo(ServerInfoRequest request)
        {
            delays();

            Console.WriteLine("ServerInfo()...");
            ServerInfoReply reply = new ServerInfoReply();
            reply.Servers.Add(new ServerInfoReply.Types.Server { Id = this.id, Url = this.URL });

            foreach (KeyValuePair<string, string> server in this.network)
            {
                reply.Servers.Add(new ServerInfoReply.Types.Server { Id = server.Key, Url = server.Value });
            }

            foreach (Partition p in this.partitions.Values)
            {
                ServerInfoReply.Types.Partition partition = new ServerInfoReply.Types.Partition { Name = p.name, Master = p.masterID };
                foreach (string server in p.replicas.Keys)
                {
                    partition.ServerIds.Add(server);
                }

                reply.Partition.Add(partition);
            }

            return reply;
        }


        /*
         * Writes an object in all servers of a partition
         */
        public WriteReply write(WriteRequest request)
        {
            delays();

            Console.WriteLine("Write()...");
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

        /*
         * Sends client the value of a specific object, if it is present in the server
         */
        public ReadReply read(ReadRequest request)
        {
            delays();

            Console.WriteLine("Read()...");

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

        /*
         * Sets up a new partition and sends the partition's information to the other servers
         */
        public ListServerReply listServer(ListServerRequest request)
        {
            delays();

            Console.WriteLine("ListServer()...");
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

        /*
         * Sends client objects of asked partitions present in the server
         */
        public ListGlobalReply listGlobal(ListGlobalRequest request)
        {
            delays();

            Console.WriteLine("ListServer()...");
            ListGlobalReply reply = new ListGlobalReply();
            List<string> neededPartitions = request.PartitionIds.ToList();
            
            //TO DO: check if partitionsIds received exist in partitions stored in server
            foreach (string name in neededPartitions)
            {
                Partition p = partitions[name];
                ListGlobalReply.Types.ListPartition list = new ListGlobalReply.Types.ListPartition { PartitionId = name };
                foreach (string objectId in p.objects.Keys)
                {
                    list.ObjectId.Add(objectId);
                }
                reply.Partitions.Add(list);
            }

            return reply;
        }

        //
        //Server-Server Communication
        //

        public LockObjectReply lockObject(LockObjectRequest request)        //TO DO: handle failures
        {
            delays();

            Console.WriteLine("LockObject()...");
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
            delays();

            Console.WriteLine("WriteObject()...");

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
            Console.WriteLine("Handshake()...");

            HandshakeReply reply = new HandshakeReply();
            foreach (KeyValuePair<string, string> server in network)
            {
                reply.Network.Add(new HandshakeReply.Types.Info { Id = server.Key, Url = server.Value });
            }

            network.Add(request.Id, request.Url);
            return reply;
        }

        public RegisterReply register(RegisterRequest request)
        {
            Console.WriteLine("Register()...");

            network.Add(request.Id, request.Url);
            return new RegisterReply();
        }

        public SharePartitionReply sharePartition(SharePartitionRequest request)
        {
            delays();

            Console.WriteLine("SharePartition()...");
            string partition_name = request.Name;
            string master_id = request.Ids.First();
            string master_url = network[master_id];

            List<string> server_ids = request.Ids.ToList();
            bool own = server_ids.Contains(this.id);
            Partition p = new Partition(partition_name, master_id, master_url, own);

            foreach (string id in server_ids)
            {
                p.addId(id, this.id == id ? this.URL : network[id]);
            }

            this.partitions.Add(partition_name, p);

            return new SharePartitionReply();
        }

        public void delays()
        {
            //Any incoming message is delayed by a predefined amount
            Thread.Sleep(delay);

            //If server is frozen, the message will be delayed until server unfreezes
            if (frozen)
            {
                lock (key)
                {
                    Monitor.Wait(key);
                    Monitor.Pulse(key);
                }
            }
        }
    }
}