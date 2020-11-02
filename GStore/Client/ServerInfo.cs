using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Linq;
using Grpc.Core;
using Client.Exceptions;
using GStore;

namespace Client
{
    sealed class ServerInfo
    {
        static ServerInfo serverInfo = null;

        static readonly int backoffTime = 5000;

        readonly Dictionary<string, string> serverURL = new Dictionary<string, string>();                                       // <serverId, URL>

        readonly Dictionary<string, string> masterURL = new Dictionary<string, string>();                                       // <partitionId, URL>

        readonly Dictionary<string, List<string>> serverReplicas = new Dictionary<string, List<string>>();                      // <URL, partitionIds>

        Dictionary<string, GStore.GStore.GStoreClient> channels = new Dictionary<string, GStore.GStore.GStoreClient>();         // <URL, channel>

        public string CurrentServerURL { get; set; }
        public bool ExecFinish { get; set; }
        public string UserName { get; set; }

        public static ServerInfo Instance()
        {
            if (serverInfo == null)
                return (serverInfo = new ServerInfo());
            else 
                return serverInfo;
        }

        public string GetMasterURLByPartitionId(string partitionId)
        {
            string value;
            if (masterURL.TryGetValue(partitionId, out value))
                return value;
            else return null;
        }
        public List<string> GetURLs()
        {
            return serverURL.Values.ToList();
        }
        public List<string> GetPartitionIds()
        {
            return masterURL.Keys.ToList();
        }
        public List<string> GetPartitionsByURL(string url)
        {
            List<string> value;
            if (serverReplicas.TryGetValue(url, out value))
                return value;
            else return null;
        }
        public string GetURLByServerId(string serverId)
        {
            string value;
            if (serverURL.TryGetValue(serverId, out value))
                return value;
            else return null;
        }
        public GStore.GStore.GStoreClient GetChannel(string url)
        {
            GStore.GStore.GStoreClient client;

            CurrentServerURL = url;

            if (channels.TryGetValue(url, out client))
                return client;

            GrpcChannel channel = GrpcChannel.ForAddress(CurrentServerURL);
            client = new GStore.GStore.GStoreClient(channel);
            channels.Add(CurrentServerURL, client);

            return client;
        }

        public void AddServerURL(string serverId, string url)
        {
            serverURL.Add(serverId, url);
        }
        public List<string> GetURLsWithPartitionId(string partitionId)
        {
            List<string> urls = new List<string>();

            foreach (KeyValuePair<string, List<string>> entry in serverReplicas)
                if (entry.Value.Contains(partitionId))
                    urls.Add(entry.Key);

            return urls;
        }

        public void RegisterPartition(string name, string masterId, List<string> serverIds) {
            string url = GetURLByServerId(masterId);
            if (url == null)
                throw new NonExistentServerException(String.Format("Server with id \"{0}\" not found.", masterId));

            masterURL.Add(name, url);

            foreach (string serverId in serverIds) {
                url = GetURLByServerId(serverId);
                if (url == null)
                    throw new NonExistentServerException(String.Format("Server with id \"{0}\" not found.", serverId));

                List<string> partitionIds = GetPartitionsByURL(url);
                if (partitionIds == null)
                    serverReplicas.Add(url, new List<string>() { name } );
                else partitionIds.Add(name);
            }
        }

        public void GetServerInfo()
        {
            Random random = new Random();
            List<string> urls = serverURL.Values.ToList();

            //Drop current network information
            serverURL.Clear();
            masterURL.Clear();
            serverReplicas.Clear();

            while (true)
            {
                //Get random server from the list of known servers
                string url = urls[random.Next(urls.Count)];

                Console.WriteLine("Contacting server with URL \"{0}\" for info about the network...", url);

                try
                {
                    GStore.GStore.GStoreClient client = GetChannel(url);

                    var response = client.ServerInfo(new ServerInfoRequest { });

                    foreach (var value in response.Servers)
                        serverURL.Add(value.Id, value.Url);

                    foreach (var value in response.Partition)
                        RegisterPartition(value.Name, value.Master, value.ServerIds.ToList());

                    Console.WriteLine("Information about the network obtained.");
                    return;
                }
                catch (RpcException e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\".\nRetrying in {2}ms...", url, e.StatusCode.ToString(), backoffTime));
                    Console.WriteLine("Server with URL \"{0}\" failed with status \"{1}\". Retrying in {2}ms...\n", url, e.StatusCode.ToString(), backoffTime);
                    System.Threading.Thread.Sleep(backoffTime);
                }
            }
        }
    }
}
